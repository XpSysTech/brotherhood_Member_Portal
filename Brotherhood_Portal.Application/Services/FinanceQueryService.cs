using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Finance.Query;
using Brotherhood_Portal.Domain.DTOs.Finance.ValueObject;
using Brotherhood_Portal.Domain.Entities;

/// <summary>
/// QUERY / READ-ONLY SERVICE
/// 
/// [1] PURPOSE
/// Provides read-only financial reporting and aggregation logic
/// for members and the fund.
/// 
/// [2] RESPONSIBILITIES
/// - Member financial summaries
/// - Fund-wide summaries
/// - Deposit history retrieval
/// - Pending deposit retrieval
/// - DTO projection and mapping
/// 
/// [3] RULES
/// - No database writes
/// - No state changes
/// - No approval mutations
/// - Purely reporting and data projection
/// 
/// [4] ARCHITECTURE
/// - Depends only on IFinanceQueryRepository
/// - Follows CQRS (Query side only)
/// </summary>
public sealed class FinanceQueryService
{
    private readonly IFinanceQueryRepository _repository;

    public FinanceQueryService(IFinanceQueryRepository repository)
    {
        _repository = repository;
    }

    #region Member Summary

    /// <summary>
    /// [1] PURPOSE
    /// Returns a financial summary for a specific member.
    ///
    /// [2] BUSINESS RULES
    /// - Only approved finances are included.
    /// - Ownership is calculated against total approved fund savings.
    /// - Monthly values are filtered by year and month.
    ///
    /// [3] RESPONSE
    /// Returns:
    /// - Lifetime totals
    /// - Monthly totals
    /// - Ownership percentage of the fund
    ///
    /// [4] DESIGN NOTES
    /// - Uses repository-level approved filters.
    /// - Avoids modifying any persisted state.
    /// </summary>
    public async Task<MemberFinanceSummaryDTO?> GetMemberFinanceSummaryAsync(
        string memberId, int year, int month)
    {
        var approved = await _repository
            .GetApprovedFinancesByMemberAsync(memberId);

        var fundTotals = await _repository
            .GetApprovedFinancesAsync();

        var totalSavings = approved.Sum(f => f.SavingsAmount);
        var totalOps = approved.Sum(f => f.OpsContributionAmount);

        var monthly = approved
            .Where(f => f.DepositDate.Year == year &&
                        f.DepositDate.Month == month);

        var fundTotalSavings = fundTotals.Sum(f => f.SavingsAmount);

        return new MemberFinanceSummaryDTO
        {
            MemberId = memberId,
            TotalSavings = totalSavings,
            TotalOpsContribution = totalOps,
            Year = year,
            Month = month,
            MonthlySavings = monthly.Sum(f => f.SavingsAmount),
            MonthlyOpsContribution = monthly.Sum(f => f.OpsContributionAmount),
            OwnershipPercentageOfFund =
                fundTotalSavings == 0
                    ? 0
                    : (totalSavings / fundTotalSavings) * 100
        };
    }

    #endregion


    #region Fund Summary

    /// <summary>
    /// [1] PURPOSE
    /// Returns an aggregate summary of the entire fund.
    ///
    /// [2] BUSINESS RULES
    /// - Only approved deposits are included.
    /// - Monthly aggregates are derived from grouped repository data.
    ///
    /// [3] RESPONSE
    /// Returns:
    /// - Lifetime totals
    /// - Active & total member counts
    /// - Current month snapshot
    /// - Historical monthly aggregates
    ///
    /// [4] DESIGN NOTES
    /// - MonthlyHistory uses repository-level grouped aggregation.
    /// - Prevents recalculating historical summaries manually.
    /// </summary>
    public async Task<FundFinanceSummaryDTO> GetFundFinanceSummaryAsync(int year, int month)
    {
        var approved = await _repository.GetApprovedFinancesAsync();
        var aggregates = await _repository.GetMonthlyAggregatesAsync();

        var monthly = approved
            .Where(f => f.DepositDate.Year == year &&
                        f.DepositDate.Month == month);

        return new FundFinanceSummaryDTO
        {
            TotalSavings = approved.Sum(f => f.SavingsAmount),
            TotalOpsContribution = approved.Sum(f => f.OpsContributionAmount),

            TotalMembers = await _repository.GetTotalMemberCountAsync(),
            ActiveMembers = await _repository.GetActiveMemberCountAsync(),

            CurrentMonth = new MonthlyFinanceSummaryDTO
            {
                Year = year,
                Month = month,
                TotalSavings = monthly.Sum(f => f.SavingsAmount),
                TotalOpsContribution = monthly.Sum(f => f.OpsContributionAmount),
                ContributingMemberCount =
                    monthly.Select(f => f.MemberId).Distinct().Count()
            },

            MonthlyHistory = aggregates
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Select(x => new MonthlyFinanceSummaryDTO
                {
                    Year = x.Year,
                    Month = x.Month,
                    TotalSavings = x.TotalSavings,
                    TotalOpsContribution = x.TotalOpsContribution,
                    ContributingMemberCount = x.ContributingMemberCount
                })
                .ToList()
        };
    }

    #endregion


    #region Pending Deposits

    /// <summary>
    /// [1] PURPOSE
    /// Retrieves all deposits currently awaiting approval.
    ///
    /// [2] BUSINESS RULES
    /// - Only deposits with Status == Pending are returned.
    /// - Includes full approval history for transparency.
    ///
    /// [3] RESPONSE
    /// Returns:
    /// - FinanceId
    /// - Member identity
    /// - Contribution amounts
    /// - Approval count
    /// - Created timestamp
    /// - Approval history
    ///
    /// [4] DESIGN NOTES
    /// - Mapping is delegated to MapToPendingDepositDto.
    /// - Keeps method readable and minimal.
    /// </summary>
    public async Task<List<PendingDepositDto>> GetPendingDepositsAsync()
    {
        var pending = await _repository
            .GetPendingFinancesWithApprovalsAsync();

        return pending
            .OrderByDescending(f => f.CreatedAt)
            .Select(MapToPendingDepositDto)
            .ToList();
    }

    #endregion


    #region Member Deposit History

    /// <summary>
    /// [1] PURPOSE
    /// Retrieves the complete deposit ledger for a specific member.
    ///
    /// [2] BUSINESS RULES
    /// - Includes Pending, Approved, and Rejected records.
    /// - Includes creator and approval metadata.
    /// - Does not modify system state.
    ///
    /// [3] RESPONSE
    /// Returns chronological deposit history including:
    /// - FinanceId
    /// - Contribution amounts
    /// - Status
    /// - Approval count
    /// - Created & Approved timestamps
    /// - Approval audit history
    ///
    /// [4] DESIGN NOTES
    /// - Uses repository method including approvals and users.
    /// - Mapping isolated in helper method.
    /// </summary>
    public async Task<List<MemberDepositHistoryDto>> GetMemberDepositHistoryAsync(string memberId)
    {
        var finances = await _repository
            .GetFinancesByMemberWithApprovalsAsync(memberId);

        return finances
            .OrderByDescending(f => f.CreatedAt)
            .Select(MapToDepositHistoryDto)
            .ToList();
    }

    #endregion


    #region Mapping Helpers

    /// <summary>
    /// Maps Finance entity to PendingDepositDto.
    /// </summary>
    private static PendingDepositDto MapToPendingDepositDto(Finance finance)
        => new()
        {
            FinanceId = finance.Id!.Value,
            MemberId = finance.MemberId,
            MemberDisplayName = finance.Member.DisplayName!,
            SavingsAmount = finance.SavingsAmount,
            OpsContributionAmount = finance.OpsContributionAmount,
            InvoiceNo = finance.InvoiceNo!,
            ApprovalCount = finance.ApprovalCount,
            CreatedAt = finance.CreatedAt,
            Approvals = finance.Approvals
                .Select(a => new FinanceApprovalDto
                {
                    UserId = a.UserId,
                    DisplayName = a.User.DisplayName!,
                    Role = a.Role,
                    ApprovedAt = a.ApprovedAt
                })
                .ToList()
        };

    /// <summary>
    /// Maps Finance entity to MemberDepositHistoryDto.
    /// </summary>
    private static MemberDepositHistoryDto MapToDepositHistoryDto(Finance finance)
        => new()
        {
            FinanceId = finance.Id!.Value,
            InvoiceNo = finance.InvoiceNo!,
            SavingsAmount = finance.SavingsAmount,
            OpsContributionAmount = finance.OpsContributionAmount,
            Status = finance.Status.ToString(),
            CreatedAt = finance.CreatedAt,
            ApprovedAt = finance.ApprovedAt,
            CreatedByUserId = finance.CreatedByUserId,
            ApprovedByUserId = finance.ApprovedByUserId,
            ApprovalCount = finance.ApprovalCount,
            Approvals = finance.Approvals
                .Select(a => new FinanceApprovalDto
                {
                    UserId = a.UserId,
                    DisplayName = a.User.DisplayName!,
                    Role = a.Role,
                    ApprovedAt = a.ApprovedAt
                })
                .ToList()
        };

    #endregion

    #region Get Member Monthly Totals Async

    #endregion
}
