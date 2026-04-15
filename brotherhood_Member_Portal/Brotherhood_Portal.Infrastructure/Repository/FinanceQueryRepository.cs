using Brotherhood_Portal.Application.Aggregates;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    /// <summary>
    /// FINANCE QUERY REPOSITORY
    /// 
    /// [1] PURPOSE
    /// Read-only data access layer for finance-related queries.
    /// 
    /// [2] RESPONSIBILITIES
    /// - Retrieve approved finance records
    /// - Retrieve member-specific finance data
    /// - Retrieve pending deposits with approval chains
    /// - Provide aggregate reporting data
    /// - Provide counts for dashboard summaries
    ///
    /// [3] ARCHITECTURAL ROLE
    /// - This repository belongs to the QUERY side of the system
    /// - It must NEVER modify state
    /// - It must NEVER perform business logic
    /// - It must NEVER apply deposits or approvals
    ///
    /// [4] DESIGN PRINCIPLES
    /// - Uses AsNoTracking() for performance on read operations
    /// - Includes navigation properties only when required
    /// - Keeps EF-specific logic isolated from Application layer
    ///
    /// This follows CQRS principles:
    /// Commands (mutations) are handled elsewhere.
    /// Queries (reads) are handled here.
    /// </summary>
    public class FinanceQueryRepository : IFinanceQueryRepository
    {
        private readonly AppDBContext _context;

        public FinanceQueryRepository(AppDBContext context)
        {
            _context = context;
        }

        #region Approved Finance Queries

        /// <summary>
        /// Retrieves all approved finance records.
        /// 
        /// Used for:
        /// - Fund summary calculations
        /// - Ownership percentage calculations
        /// - Reporting
        /// </summary>
        public async Task<IReadOnlyList<Finance>> GetApprovedFinancesAsync()
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f => f.Status == FinanceStatus.Approved)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all approved finance records for a specific member.
        /// 
        /// Used for:
        /// - Member summary calculations
        /// - Member ownership percentage
        /// - Monthly member reports
        /// </summary>
        public async Task<IReadOnlyList<Finance>> GetApprovedFinancesByMemberAsync(string memberId)
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f =>
                    f.MemberId == memberId &&
                    f.Status == FinanceStatus.Approved)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves approved finances filtered by year and month.
        /// 
        /// Used for:
        /// - Monthly fund reporting
        /// - Historical dashboard analytics
        /// </summary>
        public async Task<IReadOnlyList<Finance>> GetApprovedFinancesByMonthAsync(int year, int month)
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f =>
                    f.Status == FinanceStatus.Approved &&
                    f.DepositDate.Year == year &&
                    f.DepositDate.Month == month)
                .ToListAsync();
        }

        #endregion


        #region Member Counts

        /// <summary>
        /// Returns number of active members.
        /// 
        /// Used in dashboard reporting.
        /// </summary>
        public Task<int> GetActiveMemberCountAsync()
            => _context.Members.CountAsync(m => m.IsActive);

        /// <summary>
        /// Returns total number of members in the system.
        /// </summary>
        public Task<int> GetTotalMemberCountAsync()
            => _context.Members.CountAsync();

        #endregion


        #region Aggregates

        /// <summary>
        /// Builds monthly aggregate summaries of approved finances.
        /// 
        /// Produces:
        /// - Total savings per month
        /// - Total operations contribution per month
        /// - Distinct contributing member count
        /// 
        /// Used for:
        /// - Fund history charts
        /// - Dashboard analytics
        /// </summary>
        public async Task<IReadOnlyCollection<MonthlyFinanceAggregate>> GetMonthlyAggregatesAsync()
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f => f.Status == FinanceStatus.Approved)
                .GroupBy(f => new { f.DepositDate.Year, f.DepositDate.Month })
                .Select(g => new MonthlyFinanceAggregate
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSavings = g.Sum(f => f.SavingsAmount),
                    TotalOpsContribution = g.Sum(f => f.OpsContributionAmount),
                    ContributingMemberCount = g
                        .Select(f => f.MemberId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync();
        }

        #endregion


        #region Detailed Finance Views (Includes Navigation Properties)

        /// <summary>
        /// Retrieves all finance records for a specific member,
        /// including:
        /// - Member information
        /// - Approval chain
        /// - Approving users
        ///
        /// Used for:
        /// - Member deposit history screens
        /// - Admin auditing
        /// - Detailed financial traceability
        ///
        /// IMPORTANT:
        /// This method intentionally loads navigation properties.
        /// It is heavier than summary queries.
        /// </summary>
        public async Task<List<Finance>> GetFinancesByMemberWithApprovalsAsync(string memberId)
        {
            return await _context.Finances
                .AsNoTracking()
                .Include(f => f.Member)
                .Include(f => f.Approvals)
                    .ThenInclude(a => a.User)
                .Where(f => f.MemberId == memberId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all pending deposits including approval chains.
        ///
        /// Used for:
        /// - Admin / Moderator approval dashboards
        /// - Transparency on who approved what
        ///
        /// Only deposits with Status == Pending are returned.
        /// </summary>
        public async Task<List<Finance>> GetPendingFinancesWithApprovalsAsync()
        {
            return await _context.Finances
                .AsNoTracking()
                .Include(f => f.Member)
                .Include(f => f.Approvals)
                    .ThenInclude(a => a.User)
                .Where(f => f.Status == FinanceStatus.Pending)
                .ToListAsync();
        }

        #endregion
    }
}
