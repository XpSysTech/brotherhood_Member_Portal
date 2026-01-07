using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDBContext _dbcontext;
        public AppUserRepository(AppDBContext dbContext) 
        {
            _dbcontext = dbContext;
        }

        /*Create User*/
        public async Task<bool> CreateUserAsync(AppUser user)
        {
            await _dbcontext.Users.AddAsync(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        /*Delete User*/
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _dbcontext.Users.FindAsync(id);
            if (user == null) return false;

            _dbcontext.Users.Remove(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        /*Get User By Email*/
        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        /*Get User By ID*/
        public async Task<AppUser?> GetUserByIdAsync(string id)
        {
            var user = await _dbcontext.Users.FindAsync(id);
            return user;
        }

        /*Get All App Users*/
        public async Task<IEnumerable<AppUser>> GetAllAppUsersAsync()
        {
            return await _dbcontext.Users.ToListAsync();
        }

        /*Update User*/
        public async Task<bool> UpdateUserAsync(AppUser user)
        {
            _dbcontext.Users.Update(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        //public async Task<bool> UserExists(string email)
        //{
        //    return await _dbcontext.Users.AnyAsync(u => u.Email == email);
        //}
    }
}
