using System.Data;

namespace Brotherhood_Portal.Domain.Entities
{
    public class Finance
    {
        public int? Id { get; set; }
        public DateTime DepositDate { get; set; } = DateTime.UtcNow;
        public decimal SavingsAmount { get; set; } // Represents one deposit transaction
        public decimal OpsContributionAmount { get; set; } // Represents one deposit transaction
        public string? Description { get; set; }

        // Invoice
        public string InvoiceNo { get; set; } = null!;

        // Auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedByUserId { get; set; } = null!;

        // Status and Approval
        public FinanceStatus Status { get; set; } = FinanceStatus.Pending; // Default status is Pending
        public int ApprovalCount { get; set; } = 0;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? AppliedAt { get; set; } // Prevents double balance updates Timestamp when the deposit was applied to member's balance
        public bool IsApproved => ApprovedAt != null; // This property returns true if ApprovedAt has a value (i.e., the deposit is approved)

        //Foreign Key
        public string MemberId { get; set; } = null!;

        //Navigation Property
        public Member Member { get; set; } = null!;
        public ICollection<FinanceApproval> Approvals { get; set; } = new List<FinanceApproval>();
}

    //Used to track the status of a finance record
    public enum FinanceStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
