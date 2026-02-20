using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    /// <summary>
    /// Repository responsible for persistence operations related to Member entities.
    ///
    /// PURPOSE:
    /// - Encapsulates data access logic for Member aggregates.
    /// - Provides read and write operations for Member and related Photos.
    ///
    /// RESPONSIBILITIES:
    /// - Retrieve members by id.
    /// - Retrieve all members.
    /// - Retrieve photos associated with a member.
    /// - Mark member entities for update.
    /// - Persist changes to the database.
    ///
    /// RULES:
    /// - No business logic.
    /// - No validation rules.
    /// - No authorization decisions.
    /// - No DTO mapping.
    ///
    /// ARCHITECTURE:
    /// - Persistence-only layer.
    /// - EF Core tracking behavior is relied upon for updates.
    /// </summary>
    public class MemberRepository(AppDBContext appDBContext) : IMemberRepository
    {

        #region Get Member By Id

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a single Member entity by its unique identifier.
        ///
        /// [2] BEHAVIOR
        /// - Uses EF Core FindAsync for primary key lookup.
        /// - Returns null if not found.
        ///
        /// [3] NOTES
        /// - Does NOT include navigation properties (e.g., Photos).
        /// - For eager loading, a separate query method should be created.
        /// </summary>
        public async Task<Member?> GetMemberByIdAsync(string userId)
        {
            return await appDBContext.Members
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == userId);
        }

        #endregion


        #region Get All Members

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves all Member records.
        ///
        /// [2] BEHAVIOR
        /// - Returns full Member entities.
        /// - No filtering or projection applied.
        ///
        /// [3] WARNING
        /// This method loads all members into memory.
        /// Use cautiously in large datasets.
        /// Prefer pagination in production systems.
        /// </summary>
        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await appDBContext.Members
                .ToListAsync();
        }

        #endregion


        #region Get Photos For Member

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves all Photo entities associated with a specific member.
        ///
        /// [2] BEHAVIOR
        /// - Filters by MemberId.
        /// - Uses SelectMany to flatten photo collections.
        ///
        /// [3] NOTES
        /// - Returns an empty list if no photos exist.
        /// - Does not perform moderation or status filtering.
        /// </summary>
        public async Task<IReadOnlyList<Photo>> GetPhotosForMembersAsync(string memberId)
        {
            return await appDBContext.Members
                .Where(m => m.Id == memberId)
                .SelectMany(m => m.Photos)
                .ToListAsync();
        }

        #endregion


        #region Update Member

        /// <summary>
        /// [1] PURPOSE
        /// Marks a Member entity as modified within the DbContext.
        ///
        /// [2] BEHAVIOR
        /// - Sets EntityState to Modified.
        /// - Does NOT immediately persist changes.
        ///
        /// [3] IMPORTANT
        /// SaveAllAsync() must be called to commit changes.
        ///
        /// [4] CONCURRENCY NOTE
        /// If concurrency tokens (RowVersion) are implemented,
        /// EF Core will enforce optimistic concurrency on SaveChangesAsync.
        /// </summary>
        public void Update(Member member)
        {
            appDBContext.Entry(member).State = EntityState.Modified;
        }

        #endregion


        #region Save Changes

        /// <summary>
        /// [1] PURPOSE
        /// Commits all tracked changes in the DbContext to the database.
        ///
        /// [2] RETURNS
        /// True if one or more rows were affected.
        ///
        /// [3] USAGE
        /// Should be coordinated by Application layer.
        /// Repository should not decide transaction boundaries.
        /// </summary>
        public async Task<bool> SaveAllAsync()
        {
            return await appDBContext.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
