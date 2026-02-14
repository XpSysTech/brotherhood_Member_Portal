using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Finance.ValueObject
{
    public class FinanceApprovalDto
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime ApprovedAt { get; set; }
    }

}
