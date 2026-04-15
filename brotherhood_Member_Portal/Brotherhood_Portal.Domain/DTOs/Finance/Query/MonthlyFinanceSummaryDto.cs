namespace Brotherhood_Portal.Domain.DTOs.Finance.Query
{
    /// <summary>
    /// Represents aggregated finance data for a specific month.
    /// Used for reporting, charts, and trend analysis.
    /// </summary>
    public sealed class MonthlyFinanceSummaryDTO
    {
        // Time period
        public int Year { get; init; }
        public int Month { get; init; }

        // Aggregated contributions
        public decimal TotalSavings { get; init; }
        public decimal TotalOpsContribution { get; init; }

        // Participation
        public int ContributingMemberCount { get; init; }

        // Optional growth indicators
        public decimal SavingsGrowthPercentage { get; init; }
        public decimal OpsGrowthPercentage { get; init; }

        // Metadata
        public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;
    }
}
