using System;
using System.Collections.Generic;
using System.Linq;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Interfaces
{
    public interface IAppUserRepository
    {
        //Task<bool> UserExists(string email);
        Task<bool> CreateUserAsync(AppUser user);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<AppUser>> GetAllAppUsersAsync();
        Task<AppUser?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(AppUser user);
        Task<bool> DeleteUserAsync(string id);
    }
}
