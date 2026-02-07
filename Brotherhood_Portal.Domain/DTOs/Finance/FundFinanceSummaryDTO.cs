namespace Brotherhood_Portal.Domain.DTOs.Finance
{
    /// <summary>
    /// Read-only financial summary for the entire fund.
    /// Represents total contributions, ownership context,
    /// and investment performance at fund level.
    /// </summary>
    public sealed class FundFinanceSummaryDTO
    {
        // Fund totals (lifetime)
        public decimal TotalSavings { get; init; }
        public decimal TotalOpsContribution { get; init; }

        // Member participation
        public int TotalMembers { get; init; }
        public int ActiveMembers { get; init; }

        // Monthly snapshot (current or requested)
        public MonthlyFinanceSummaryDTO CurrentMonth { get; init; } = null!;

        // Historical trend (optional but powerful)
        public IReadOnlyCollection<MonthlyFinanceSummaryDTO> MonthlyHistory { get; init; }
            = Array.Empty<MonthlyFinanceSummaryDTO>();

        // Investment performance (future-ready)
        public decimal TotalInvestedAmount { get; init; }
        public decimal TotalInvestmentReturns { get; init; }
        public decimal NetFundValue { get; init; }

        // Metadata
        public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;
    }
}
