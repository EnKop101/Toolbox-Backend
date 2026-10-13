using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Toolbox_Library.DTO;
using Toolbox_Library.Enums;
using Toolbox_Library.Models;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Utilities.Encryption.Interface;

namespace Toolbox_Library.Services.Interfaces
{
    public interface IUserService
    {
        public Task Login(LoginDTO dto);
        public Task Add(CreateUserDTO dto);
        public Task Delete(Guid targetId, UserRole currentUserRole);
        public Task UpdateUser(UpdateUserDTO dto, Guid currentUserId);
        public Task ChangePassword(ChangePassword dto, Guid currentUserId);
        public Task<UserDTO> Get(Guid targetId, Guid currenUserId, UserRole currentUserRole);
        public Task<ICollection<UserDTO>> GetAllUsers();
    }
}
