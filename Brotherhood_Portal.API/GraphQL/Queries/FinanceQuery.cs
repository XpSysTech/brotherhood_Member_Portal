using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.DTOs.Finance.Query;
using HotChocolate.Authorization;
using System.Security.Claims;

namespace Brotherhood_Portal.API.GraphQL.Queries
{
    /// <summary>
    /// GRAPHQL QUERY ROOT – FINANCE DOMAIN
    ///
    /// [1] PURPOSE
    /// Exposes read-only finance data to authenticated users via GraphQL.
    ///
    /// [2] RESPONSIBILITIES
    /// - Provide member-specific financial summaries
    /// - Provide fund-level financial summaries
    /// - Enforce authorization boundaries
    /// - Delegate business logic to FinanceQueryService
    ///
    /// [3] SECURITY MODEL
    /// - Class-level [Authorize] ensures only authenticated users can access.
    /// - Member queries derive memberId from JWT Claims.
    /// - Prevents users from querying other members' financial data.
    ///
    /// [4] ARCHITECTURE
    /// - Thin resolver layer
    /// - No business logic here
    /// - No database access here
    /// - All calculations handled by FinanceQueryService
    /// - Follows CQRS (Query Side)
    /// </summary>
    [Authorize]
    public class FinanceQuery
    {
        #region Member Finance Query

        /// <summary>
        /// [1] PURPOSE
        /// Returns the authenticated member’s financial summary.
        ///
        /// [2] SECURITY RULE
        /// - MemberId is derived from JWT Claims.
        /// - Prevents users from manually supplying another MemberId.
        ///
        /// [3] PARAMETERS
        /// - year: Target year for monthly calculation
        /// - month: Target month for monthly calculation
        ///
        /// [4] RETURNS
        /// - Lifetime savings totals
        /// - Lifetime ops contribution totals
        /// - Monthly savings totals
        /// - Monthly ops totals
        /// - Ownership percentage of the fund
        ///
        /// [5] DESIGN NOTES
        /// - Resolver performs identity validation only.
        /// - Business logic delegated to FinanceQueryService.
        /// </summary>
        public async Task<MemberFinanceSummaryDTO?> GetMemberFinanceSummary(
            ClaimsPrincipal user,
            int year,
            int month,
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

        #endregion


        #region Fund Finance Query

        /// <summary>
        /// [1] PURPOSE
        /// Returns aggregate financial data for the entire fund.
        ///
        /// [2] ACCESS LEVEL
        /// - Accessible to any authenticated user.
        /// - Does NOT expose individual member deposit records.
        ///
        /// [3] PARAMETERS
        /// - year: Target year
        /// - month: Target month
        ///
        /// [4] RETURNS
        /// - Total fund savings
        /// - Total fund operations contributions
        /// - Active member count
        /// - Total member count
        /// - Current month summary
        /// - Monthly historical aggregates
        ///
        /// [5] DESIGN NOTES
        /// - Uses FinanceQueryService for aggregation logic.
        /// - Safe to expose because data is aggregated, not per-member detailed.
        /// </summary>
        public async Task<FundFinanceSummaryDTO> GetFundFinanceSummary(
            int year,
            int month,
            [Service] FinanceQueryService financeQueryService)
        {
            return await financeQueryService
                .GetFundFinanceSummaryAsync(year, month);
        }

        #endregion
    }
}
