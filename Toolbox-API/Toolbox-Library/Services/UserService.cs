using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Toolbox_Library.DTO;
using Toolbox_Library.Enums;
using Toolbox_Library.Models;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities;
using Toolbox_Library.Utilities.Encryption.Interface;
using static Toolbox_Library.Utilities.Response.HttpResponseException;

namespace Toolbox_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly ISessionService _sessionService;
        public UserService(
            IGenericRepository<User> userGenericRepository,
            IEncryptionHandler encryptionHandler,
            ISessionService sessionService
        )
        {
            _userGenericRepository = userGenericRepository;
            _encryptionHandler = encryptionHandler;
            _sessionService = sessionService;
        }

        public async Task<string> Login(LoginDTO dto)
        {
            string unauthorizedMessage = "Username or Password was invalid!";

            string usernameTolower = dto.Username.ToLower();

            User user = await _userGenericRepository.Get(u => u.Username.ToLower() ==  usernameTolower)
                ?? throw new UnauthorizedException(unauthorizedMessage);

            bool passwordMatch = await _encryptionHandler.VerifyArgonHashedInput(dto.Password, user.Password);
            if (passwordMatch)
            {
                if (user.Status == UserStatus.Pending )
                {
                    throw new UnauthorizedException($"{user.Username} is currently still pending.");
                }
                else if (user.Status == UserStatus.Disabled)
                {
                    throw new UnauthorizedException($"{user.Username} is disabled.");
                }

                // Make a session etc.
                SessionData sessionData = new(user.Id.ToString(), user.Role);
                string sessionValue = JsonSerializer.Serialize(sessionData);

                string sessionKey = await _sessionService.AddSessionToDB(sessionValue);
                if (!string.IsNullOrEmpty(sessionKey))
                {
                    return sessionKey;
                }
                else
                {
                    throw new InternalErrorException("Something went wrong making a session key!");
                }
            }
            else
            {
                throw new UnauthorizedException(unauthorizedMessage);
            }
        }

        public async Task<bool> Add(CreateUserDTO dto)
        {
            string usernameToLower = dto.Username.ToLower();

            if (await _userGenericRepository.DoesValueExist(u => u.Username.ToLower() == usernameToLower))
            {
                throw new ConflictException("Username is already taken");
            }
            else
            {
                string hashedPassword = await _encryptionHandler.Argon2HashString(dto.Password);
                User user = new User
                {
                    Username = usernameToLower,
                    DisplayName = dto.DisplayName,
                    Password = hashedPassword,
                    DateCreated = DateOnly.FromDateTime(DateTime.Now),
                    LastUpdated = DateOnly.FromDateTime(DateTime.Now)
                };

                return await _userGenericRepository.Add(user);
            }
        }

        public Task<bool> ChangePassword(ChangePassword dto, Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(Guid targetId, UserRole currentUserRole)
        {
            User user = await _userGenericRepository.Get(u => u.Id == targetId)
                ?? throw new NotFoundException($"User with Id \"{targetId}\" does not exist.");

            if (user.Role == UserRole.Owner) throw new UnauthorizedException("You cannot delete the owners account.");
            else if (user.Role == UserRole.Admin && currentUserRole != UserRole.Owner) throw new UnauthorizedException("Only the Owner can delete an Admin account.");
            else return await _userGenericRepository.Delete(u => u.Id == user.Id);
        }

        public async Task<UserDTO> Get(Guid targetId, Guid currenUserId, UserRole currentUserRole)
        {
            if (targetId == currenUserId || currentUserRole == UserRole.Owner || currentUserRole == UserRole.Admin)
            {
                User user = await _userGenericRepository.Get(u => u.Id == targetId)
                    ?? throw new NotFoundException($"User with Id \"{targetId}\" does not exist.");
                return new UserDTO(user);
            }
            else throw new UnauthorizedException("You are not authorized to access this information.");
        }

        public async Task<ICollection<UserDTO>> GetAllUsers()
        {
            return (await _userGenericRepository.GetAll())
                .Select(user => new UserDTO(user)
                ).ToList();
        }

        public Task<bool> UpdateUser(UpdateUserDTO dto, Guid currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
