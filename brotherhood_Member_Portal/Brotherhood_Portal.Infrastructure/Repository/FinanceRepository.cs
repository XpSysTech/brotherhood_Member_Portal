using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    /// <summary>
    /// Repository responsible for persistence and state mutation of Finance entities.
    ///
    /// RESPONSIBILITIES:
    /// - Persist Finance records
    /// - Retrieve Finance records
    /// - Apply approved deposits to Member balances
    /// - Commit database changes
    ///
    /// RULES:
    /// - No approval decision logic (handled in Application layer)
    /// - No authorization logic
    /// - No business workflow branching
    ///
    /// IMPORTANT:
    /// This repository mutates financial balances. All state validations must be
    /// performed before calling these methods (except safety guards).
    /// </summary>
    public class FinanceRepository : IFinanceRepository
    {
        private readonly AppDBContext _dBContext;

        public FinanceRepository(AppDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        #region Add Finance Record

        /// <summary>
        /// [1] PURPOSE
        /// Adds a new Finance entity to the DbContext.
        ///
        /// [2] BEHAVIOR
        /// - Stages entity for insertion.
        /// - Does NOT automatically save changes.
        ///
        /// [3] NOTE
        /// Caller must invoke SaveChangesAsync() explicitly.
        /// </summary>
        public async Task AddAsync(Finance finance)
        {
            await _dBContext.Finances.AddAsync(finance);
        }

        #endregion


        #region Get Finance Record By Id

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a Finance record by its primary key.
        ///
        /// [2] BEHAVIOR
        /// - Includes associated Member navigation property.
        ///
        /// [3] RETURNS
        /// - Finance entity if found.
        /// - Null if record does not exist.
        ///
        /// [4] NOTE
        /// This method loads related Member for balance updates.
        /// </summary>
        public async Task<Finance?> GetByIdAsync(int id)
        {
            return await _dBContext.Finances
                .Include(f => f.Member)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        #endregion


        #region Apply Finance To Member Balance

        /// <summary>
        /// [1] PURPOSE
        /// Applies an approved Finance record to the associated Member's balance.
        ///
        /// [2] SAFETY GUARANTEES
        /// - Ensures finance is Approved.
        /// - Ensures finance has not already been applied.
        /// - Ensures Member relationship is loaded.
        ///
        /// [3] IDEMPOTENCY
        /// - If AppliedAt is already set, method exits safely.
        ///
        /// [4] SIDE EFFECTS
        /// - Increments Member.TotalSavings
        /// - Increments Member.TotalOpsContribution
        /// - Sets Finance.AppliedAt timestamp
        ///
        /// [5] EXCEPTIONS
        /// - Throws if finance is not approved.
        /// - Throws if Member relationship is missing.
        /// </summary>
        public async Task ApplyToMemberBalanceAsync(Finance finance)
        {
            // Ensure Member navigation is loaded
            if (finance.Member == null)
            {
                finance = await _dBContext.Finances
                    .Include(f => f.Member)
                    .FirstOrDefaultAsync(f => f.Id == finance.Id)
                    ?? throw new InvalidOperationException("Finance not found.");
            }

            // Validate finance approval status
            if (finance.Status != FinanceStatus.Approved)
                throw new InvalidOperationException("Cannot apply unapproved deposit.");

            // Idempotency protection
            if (finance.AppliedAt != null)
                return;

            var member = finance.Member
                ?? throw new InvalidOperationException("Finance has no associated member.");

            // Apply deposit amounts
            member.TotalSavings += finance.SavingsAmount;
            member.TotalOpsContribution += finance.OpsContributionAmount;

            finance.AppliedAt = DateTime.UtcNow;
        }

        #endregion


        #region Save Changes To Database

        /// <summary>
        /// [1] PURPOSE
        /// Commits all staged changes to the database.
        ///
        /// [2] BEHAVIOR
        /// - Executes EF Core SaveChangesAsync.
        ///
        /// [3] NOTE
        /// - Should be called explicitly after AddAsync or ApplyToMemberBalanceAsync.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _dBContext.SaveChangesAsync();
        }

        #endregion
    }
}
