using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.DTO;
using Toolbox_Library.Enums;
using Toolbox_Library.Models;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
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

        public async Task Login(LoginDTO dto)
        {
            string unauthorizedMessage = "Username or Password was invalid!";

            string usernameTolower = dto.Username.ToLower();
            string passwordTolower = dto.Password.ToLower();

            User user = await _userGenericRepository.Get(u => u.Username.ToLower() ==  usernameTolower)
                ?? throw new UnauthorizedException(unauthorizedMessage);

            bool passwordMatch = await _encryptionHandler.VerifyArgonHashedInput(passwordTolower, user.Password);
            if (passwordMatch)
            {
                // Make a session etc.
                return;
            }
            else
            {
                throw new UnauthorizedException(unauthorizedMessage);
            }
        }

        public async Task Add(CreateUserDTO dto)
        {
            string usernameToLower = dto.Username.ToLower();
            string passwordToLower = dto.Password.ToLower();

            if (!await _userGenericRepository.DoesValueExist(u => u.Username.ToLower() == usernameToLower))
            {
                throw new ConflictException("Username is already taken");
            }
            else
            {
                string hashedPassword = await _encryptionHandler.Argon2HashString(passwordToLower);
                User user = new User
                {
                    Username = usernameToLower,
                    DisplayName = dto.DisplayName,
                    Password = hashedPassword,
                    DateCreated = DateOnly.FromDateTime(DateTime.Now),
                    LastUpdated = DateOnly.FromDateTime(DateTime.Now)
                };

                await _userGenericRepository.Add(user);
                return;
            }
        }

        public Task ChangePassword(ChangePassword dto, Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid targetId, UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> Get(Guid targetId, Guid currenUserId, UserRole currentUserRole)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<UserDTO>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(UpdateUserDTO dto, Guid currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
