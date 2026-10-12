using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.DTO;
using Toolbox_Library.Enums;
using Toolbox_Library.Models;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities.Encryption.Interface;

namespace Toolbox_Library.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IEncryptionHandler _encryptionHandler;
        public UserService(IGenericRepository<User> userGenericRepository, IEncryptionHandler encryptionHandler)
        {
            _userGenericRepository = userGenericRepository;
            _encryptionHandler = encryptionHandler;
        }
        public Task Add(CreateUserDTO dto)
        {
            throw new NotImplementedException();
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
