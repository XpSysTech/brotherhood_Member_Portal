namespace Brotherhood_Portal.Domain.DTOs.Finance.Command
{
    public sealed class AddDepositRequestDto
    {
        /*
            Used when creating a new member deposit.
            This represents an incoming API request (Command DTO).
        */

        public string MemberId { get; set; } = null!;

        public string MemberDisplayName { get; set; } = default!;

        public decimal SavingsAmount { get; set; }

        public decimal OpsContribution { get; set; }

        public string? Description { get; set; }
    }

}
