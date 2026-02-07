using Brotherhood_Portal.Application.Aggregates;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    public class FinanceQueryRepository : IFinanceQueryRepository
    {
        /*
            - QUERY / READ-ONLY REPOSITORY
            - Responsibilities:
                - Data retrieval for finance-related queries
                - Support for financial summaries and reports
         */

        private readonly AppDBContext _context;

        public FinanceQueryRepository(AppDBContext context)
        {
            _context = context;
        }

        #region Get Approved Finances
        /*
            - Summary:
                - Retrieves a list of all approved finance records from the database.
                - Where the finance status is 'Approved'.
                - Returns:
                    - A read-only list of Finance entities that have been approved.s
         */
        public async Task<IReadOnlyList<Finance>> GetApprovedFinancesAsync()
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f => f.Status == FinanceStatus.Approved)
                .ToListAsync();
        }
        #endregion


        #region Get Approved Finance By Id
        /*
            - Summary:
                - Retrieves a specific approved finance record by its unique identifier.
                - Parameters:
                    - id: The unique identifier of the finance record to retrieve.
                - Returns:
                    - The Finance entity with the specified ID if it is approved; otherwise, null.
         */
        public async Task<IReadOnlyList<Finance>> GetApprovedFinancesByMemberAsync(string memberId)
        {
            return await _context.Finances
                .AsNoTracking()
                .Where(f =>
                    f.MemberId == memberId &&
                    f.Status == FinanceStatus.Approved)
                .ToListAsync();
        }
        #endregion


        #region Get Approved Finances by Date
        /*
            - Summary:
                - Retrieves a list of approved finance records for a specific month and year.
                - Parameters:
                    - year: The year to filter the finance records.
                    - month: The month to filter the finance records.
                - Returns:
                    - A read-only list of Finance entities that have been approved within the specified month and year.
         */
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


        #region Get Active Member Count
        /*
            - Summary:
                - Retrieves the count of active members in the database.
                - Returns:
                    - An integer representing the number of active members.
         */
        public Task<int> GetActiveMemberCountAsync()
            => _context.Members.CountAsync(m => m.IsActive);

        #endregion


        #region Get Total Member Count
        /*
            - Summary:
                - Retrieves the total count of members in the database.
                - Returns:
                    - An integer representing the total number of members.
         */
        public Task<int> GetTotalMemberCountAsync()
            => _context.Members.CountAsync();

        #endregion

        #region Get Monthly Aggregates
        /*
            - Summary:
                - Retrieves monthly aggregates of approved finances.
                - Groups finances by year and month, calculating total savings, total operations contributions, and the count of contributing members.
                - Returns:
                    - A read-only collection of MonthlyFinanceAggregate objects representing the aggregated data for each month.
         */
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
                    ContributingMemberCount = g.Select(f => f.MemberId).Distinct().Count()
                })
                .ToListAsync();
        }
        #endregion
    }
}
