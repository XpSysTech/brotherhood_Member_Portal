namespace Brotherhood_Portal.Domain.DTOs.Finance
{
    public class AddDepositDto
    {
        /*
            - This DTO is used to transfer data when adding a deposit for a member.
         */
        public string MemberId { get; set; } = null!;
        public decimal SavingsAmount { get; set; }
        public decimal OpsContribution { get; set; }
        public string? Description { get; set; }
    }
}
