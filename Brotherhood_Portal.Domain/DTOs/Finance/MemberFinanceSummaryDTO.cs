namespace Brotherhood_Portal.Domain.DTOs.Finance
{
    /// <summary>
    /// Read-only financial summary for a single member.
    /// Represents the member's contribution position within the fund.
    /// </summary>
    public sealed class MemberFinanceSummaryDTO
    {
        // Identity
        public string MemberId { get; init; } = null!;
        public string? DisplayName { get; init; }

        // Lifetime totals
        public decimal TotalSavings { get; init; }
        public decimal TotalOpsContribution { get; init; }

        // Monthly snapshot
        public int Year { get; init; }
        public int Month { get; init; }
        public decimal MonthlySavings { get; init; }
        public decimal MonthlyOpsContribution { get; init; }

        // Ownership
        /// <summary>
        /// Percentage ownership of the total fund savings.
        /// Example: 12.45 means 12.45%
        /// </summary>
        public decimal OwnershipPercentageOfFund { get; init; }

        /// <summary>
        /// Percentage ownership of the monthly fund savings.
        /// </summary>
        public decimal MonthlyOwnershipPercentage { get; init; }

        //// 🔹 Investment returns
        //public IReadOnlyCollection<MemberInvestmentReturnDTO> InvestmentReturns { get; init; }
        //    = Array.Empty<MemberInvestmentReturnDTO>();

        // Metadata
        public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;
    }
}
