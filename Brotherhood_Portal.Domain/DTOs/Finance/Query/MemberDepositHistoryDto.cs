using Brotherhood_Portal.Domain.DTOs.Finance.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Finance.Query
{
    public class MemberDepositHistoryDto
    {

        /*
            We use FinanceId
                - So that when we use this DTO on the Frontend we call
                    - Finance.FinanceId not Finance.Id
         */
        public int FinanceId { get; set; }

        public DateTime DepositDate { get; set; }

        public decimal SavingsAmount { get; set; }
        public decimal OpsContributionAmount { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; } = default!;

        public int ApprovalCount { get; set; }

        public string? InvoiceNo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string CreatedByUserId { get; set; } = default!;
        public string? ApprovedByUserId { get; set; }

        public List<FinanceApprovalDto> Approvals { get; set; } = [];

    }
}
