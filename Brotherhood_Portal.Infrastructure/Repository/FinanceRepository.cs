using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    public class FinanceRepository : IFinanceRepository
    {
        /*
            - Data access implementation for Finance entity 
            - Provides methods to add, get by id, and save changes
                - AddAsync: Adds a new Finance record
                - GetByIdAsync: Retrieves a Finance record by its ID
                - SaveChangesAsync: Commits changes to the database
            - Interacts with AppDBContexts
         */

        private readonly AppDBContext _dBContext;

        public FinanceRepository(AppDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        #region Add Finance Record
        public async Task AddAsync(Finance finance)
        {
            await _dBContext.Finances.AddAsync(finance);
        }
        #endregion

        #region Get Finance Record By Id
        public async Task<Finance?> GetByIdAsync(int id)
        {
            // Returns the finance record with the specified ID, including the associated member
            return await _dBContext.Finances
                .Include(f => f.Member)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        #endregion

        #region Apply Finance to Member Balance
        public async Task ApplyToMemberBalanceAsync(Finance finance)
        {
            if (finance.Member == null)
            {
                finance = await _dBContext.Finances
                    .Include(f => f.Member)
                    .FirstOrDefaultAsync(f => f.Id == finance.Id)
                    ?? throw new InvalidOperationException("Finance not found.");
            }

            // Ensure finance is approved
            if (finance.Status != FinanceStatus.Approved)
                throw new InvalidOperationException("Cannot apply unapproved deposit.");

            // Idempotency check
            if (finance.AppliedAt != null)
                return; // already applied (idempotency protection)

            // If Finance has no associated member, throw an exception
            var member = finance.Member
                ?? throw new InvalidOperationException("Finance has no member.");

            // Update member's total savings and ops contribution by adding to existing totals
            member.TotalSavings += finance.SavingsAmount;
            member.TotalOpsContribution += finance.OpsContributionAmount;

            finance.AppliedAt = DateTime.UtcNow;
        }
        #endregion

        #region Save Changes to Database
        public async Task SaveChangesAsync()
        {
            await _dBContext.SaveChangesAsync();
        }
        #endregion
    }
}
