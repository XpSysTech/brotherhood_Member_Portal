using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Finance.Query
{
    public sealed class AddDepositResponseDto
    {
        /*
            Returned after a deposit is created.
            Represents the result of the command.
        */

        public int FinanceId { get; set; }

        public string MemberId { get; set; } = null!;
        
        public string MemberDisplayName { get; set; } = default!;

        public string Status { get; set; } = null!;
        // "Pending" | "Approved"

        public int ApprovalCount { get; set; }

        public bool RequiresApproval { get; set; }

        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }

}
