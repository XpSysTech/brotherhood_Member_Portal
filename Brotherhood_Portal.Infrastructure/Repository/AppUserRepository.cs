using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    /// <summary>
    /// Repository responsible for persistence operations related to AppUser entities.
    ///
    /// RESPONSIBILITIES:
    /// - Direct database interaction for AppUser entity
    /// - CRUD operations
    ///
    /// RULES:
    /// - No business logic
    /// - No authorization logic
    /// - No token generation
    /// - Pure data access layer
    ///
    /// NOTE:
    /// Identity-related higher-level operations should remain in Application or API layer.
    /// </summary>
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDBContext _dbcontext;

        public AppUserRepository(AppDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        #region Create User

        /// <summary>
        /// [1] PURPOSE
        /// Persists a new AppUser entity to the database.
        ///
        /// [2] BEHAVIOR
        /// - Adds user to DbContext
        /// - Saves changes immediately
        ///
        /// [3] RETURNS
        /// - True if at least one row was affected.
        /// - False if persistence failed.
        ///
        /// [4] NOTE
        /// - Does NOT handle password hashing (handled by Identity).
        /// </summary>
        public async Task<bool> CreateUserAsync(AppUser user)
        {
            await _dbcontext.Users.AddAsync(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        #endregion


        #region Delete User

        /// <summary>
        /// [1] PURPOSE
        /// Removes a user from the database by Id.
        ///
        /// [2] BEHAVIOR
        /// - Looks up user by primary key.
        /// - If found, removes user.
        ///
        /// [3] RETURNS
        /// - True if deletion succeeded.
        /// - False if user does not exist or deletion failed.
        ///
        /// [4] WARNING
        /// - Does not cascade manual cleanup beyond EF configuration.
        /// </summary>
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _dbcontext.Users.FindAsync(id);
            if (user == null) return false;

            _dbcontext.Users.Remove(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        #endregion

        
        #region Disable User Account
        
        #endregion


        #region Get User By Email

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a user by email address.
        ///
        /// [2] BEHAVIOR
        /// - Performs case-sensitive database match (depending on DB collation).
        ///
        /// [3] RETURNS
        /// - AppUser if found.
        /// - Null if no matching record exists.
        /// </summary>
        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _dbcontext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        #endregion


        #region Get User By Id

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a user by primary key.
        ///
        /// [2] RETURNS
        /// - AppUser if found.
        /// - Null if no record exists.
        /// </summary>
        public async Task<AppUser?> GetUserByIdAsync(string id)
        {
            return await _dbcontext.Users.FindAsync(id);
        }

        #endregion


        #region Get All Users

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves all application users.
        ///
        /// [2] BEHAVIOR
        /// - Executes full table query.
        ///
        /// [3] RETURNS
        /// - IEnumerable of AppUser entities.
        ///
        /// [4] WARNING
        /// - Use carefully in large datasets.
        /// - Consider pagination in production systems.
        /// </summary>
        public async Task<IEnumerable<AppUser>> GetAllAppUsersAsync()
        {
            return await _dbcontext.Users.ToListAsync();
        }

        #endregion


        #region Update User

        /// <summary>
        /// [1] PURPOSE
        /// Updates an existing AppUser entity.
        ///
        /// [2] BEHAVIOR
        /// - Marks entity as modified.
        /// - Saves changes immediately.
        ///
        /// [3] RETURNS
        /// - True if update succeeded.
        /// - False if no changes were persisted.
        ///
        /// [4] NOTE
        /// - Caller must ensure entity state is valid.
        /// </summary>
        public async Task<bool> UpdateUserAsync(AppUser user)
        {
            _dbcontext.Users.Update(user);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        #endregion

        #region Reset User Password

        #endregion
    }
}
