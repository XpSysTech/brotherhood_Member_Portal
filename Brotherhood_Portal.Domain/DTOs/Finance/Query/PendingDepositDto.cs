using Brotherhood_Portal.Domain.DTOs.Finance.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Finance.Query
{
    public class PendingDepositDto
    {
        public int FinanceId { get; set; }
        public string MemberId { get; set; } = default!;
        public string MemberDisplayName { get; set; } = default!;
        public string? InvoiceNo { get; set; }
        public decimal SavingsAmount { get; set; }
        public decimal OpsContributionAmount { get; set; }

        public int ApprovalCount { get; set; }
        public int RequiredApprovals { get; set; } = 2;

        public DateTime CreatedAt { get; set; }

        public List<FinanceApprovalDto> Approvals { get; set; } = [];
    }

}
