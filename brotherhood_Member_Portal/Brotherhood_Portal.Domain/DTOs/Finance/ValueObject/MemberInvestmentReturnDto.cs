namespace Brotherhood_Portal.Domain.DTOs.Finance.ValueObject
{
    /// <summary>
    /// Represents a member's share of returns from a single fund investment.
    /// </summary>
    public sealed class MemberInvestmentReturnDTO
    {
        public string InvestmentId { get; init; } = null!;
        public string InvestmentName { get; init; } = null!;

        // Fund-level numbers
        public decimal TotalFundInvestment { get; init; }
        public decimal TotalFundReturn { get; init; }

        // Member-level allocation
        public decimal MemberOwnershipPercentage { get; init; }
        public decimal MemberReturnAmount { get; init; }

        // Performance
        public decimal ReturnPercentage { get; init; }

        // Status
        public bool IsRealized { get; init; }
        public DateTime? RealizedAt { get; init; }

        // Future Proofing
        public decimal TotalInvestmentReturns { get; init; }
        public decimal UnrealizedReturns { get; init; }
        public decimal RealizedReturns { get; init; }

    }
}
