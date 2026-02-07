using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.DTOs.Finance;
using HotChocolate.Authorization;
using System.Security.Claims;

namespace Brotherhood_Portal.API.GraphQL.Queries
{
    [Authorize] // Ensure only logged-in users can access these queries
    public class FinanceQuery
    {
        #region Member Finance Query
        // Get Member Finance Summary, Params are MemberId, Year, Month
        // Use the FinanceQueryService to get the data
        // Return FinanceQueryService.GetMemberFinanceSummaryAsync(memberId, year, month);
        // Example: Return data => TotalSavings, TotalOpsContribution, MonthlySavings, MonthlyOpsContribution
        public async Task<MemberFinanceSummaryDTO?> GetMemberFinanceSummary(ClaimsPrincipal user, int year, int month,
            [Service] FinanceQueryService financeQueryService,
            [Service] IMemberRepository memberRepository)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                throw new GraphQLException("User not authenticated.");

            var member = await memberRepository.GetMemberByIdAsync(userId);

            if (member is null)
                throw new GraphQLException("No member linked to this user.");

            return await financeQueryService
                .GetMemberFinanceSummaryAsync(member.Id, year, month);
        }

        // Get Member Monthly Totals, Params are MemberId, Year, Month
        // Use the FinanceQueryService to get the data
        // Return FinanceQueryService.GetMemberMonthlyTotalsAsync(memberId, year, month);
        // Example: Return data => MonthlySavingsTotal, MonthlyOpsContributionTotal

        public async Task<MonthlyFinanceSummaryDTO> GetMemberMonthlyTotals(string memberId, int year, int month,
            [Service] FinanceQueryService financeQueryService)
        {
            return await financeQueryService
                .GetMemberMonthlyTotalsAsync(memberId, year, month);
        }

        #endregion


        #region Fund Finance Query
        // Get Fund Finance Summary, Params are Year, Month
        // Use the FinanceQueryService to get the data
        // Return FinanceQueryService.GetFundFinanceSummaryAsync(year, month);
        // Example: Return data => TotalFundSavings, TotalFundOpsContribution, MonthlyFundSavings, MonthlyFundOpsContribution

        public async Task<FundFinanceSummaryDTO> GetFundFinanceSummary(int year, int month,
            [Service] FinanceQueryService financeQueryService)
        {
            return await financeQueryService
                .GetFundFinanceSummaryAsync(year, month);
        }
        #endregion

        #region Ownership Calculations
        // Later Expansion, Ignore for now
        #endregion

        #region Investment Returns
        // Later Expansion, Ignore for now
        #endregion
    }
}
