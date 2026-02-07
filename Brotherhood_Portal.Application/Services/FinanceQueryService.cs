using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Finance;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Brotherhood_Portal.Application.Services
{
    public sealed class FinanceQueryService
    {
        /*
            QUERY / READ-ONLY SERVICE

            Responsibilities:
            - Member financial summaries
            - Fund-wide financial summaries
            - Ownership calculations
            - Time-based aggregations
            - Reporting-style queries

            Rules:
            - NO database writes
            - NO state changes
            - NO approvals
        */

        private readonly IFinanceQueryRepository _financeQueryRepository;

        public FinanceQueryService(IFinanceQueryRepository repository)
        {
            _financeQueryRepository = repository;
        }

        #region Member Summaries
        public async Task<MemberFinanceSummaryDTO?> GetMemberFinanceSummaryAsync(string memberId, int year, int month)
        {
            // Retrieve all approved finances for the member
            var finances = await _financeQueryRepository.GetApprovedFinancesByMemberAsync(memberId);

            // Calculate totals for Member Savings and Ops Contribution
            var totalSavings = finances.Sum(f => f.SavingsAmount);
            var totalOps = finances.Sum(f => f.OpsContributionAmount);

            // Filter finances for the specified month and year
            var monthly = finances
                .Where(f => f.DepositDate.Year == year && f.DepositDate.Month == month)
                .ToList();

            // Retrieve fund totals for ownership calculation
            var fundTotals = await _financeQueryRepository.GetApprovedFinancesAsync();

            // Map to DTO
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
                    fundTotals.Sum(f => f.SavingsAmount) == 0
                        ? 0
                        : (totalSavings / fundTotals.Sum(f => f.SavingsAmount)) * 100
            };
        }

        #endregion


        #region Member Monthly Totals

        public async Task<MonthlyFinanceSummaryDTO> GetMemberMonthlyTotalsAsync(string memberId, int year, int month)
        {
            // Retrieve all approved finances for the member
            var finances = await _financeQueryRepository.GetApprovedFinancesByMemberAsync(memberId);

            // Filter finances for the specified month and year
            var monthly = finances
                .Where(f => f.DepositDate.Year == year && f.DepositDate.Month == month)
                .ToList();

            // Map to DTO
            return new MonthlyFinanceSummaryDTO
            {
                Year = year,
                Month = month,
                TotalSavings = monthly.Sum(f => f.SavingsAmount),
                TotalOpsContribution = monthly.Sum(f => f.OpsContributionAmount),
                ContributingMemberCount = monthly.Any() ? 1 : 0
            };
        }

        #endregion


        #region Fund Summaries
        public async Task<FundFinanceSummaryDTO> GetFundFinanceSummaryAsync(int year, int month)
        {
            // Retrieve all approved finances for the fund
            var allFinances = await _financeQueryRepository.GetApprovedFinancesAsync();

            // Filter finances for the specified month and year
            var monthlyFinances = allFinances
                .Where(f => f.DepositDate.Year == year && f.DepositDate.Month == month)
                .ToList();

            // Retrieve monthly aggregates for history
            var monthlyAggregates = await _financeQueryRepository.GetMonthlyAggregatesAsync();


            return new FundFinanceSummaryDTO
            {
                // Calculate sum of Savings and Ops Contribution
                TotalSavings = allFinances.Sum(f => f.SavingsAmount),
                TotalOpsContribution = allFinances.Sum(f => f.OpsContributionAmount),

                // Get total and active member counts
                TotalMembers = await _financeQueryRepository.GetTotalMemberCountAsync(),
                ActiveMembers = await _financeQueryRepository.GetActiveMemberCountAsync(),

                //  Calculate current month summary
                CurrentMonth = new MonthlyFinanceSummaryDTO
                {
                    Year = year,
                    Month = month,
                    TotalSavings = monthlyFinances.Sum(f => f.SavingsAmount),
                    TotalOpsContribution = monthlyFinances.Sum(f => f.OpsContributionAmount),
                    ContributingMemberCount =
                        monthlyFinances.Select(f => f.MemberId)
                                       .Distinct()
                                       .Count()
                },

                // Build monthly history
                MonthlyHistory = monthlyAggregates
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

    }
}
