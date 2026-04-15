using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Member.Query
{
    public class MemberDto
    {
        public string Id { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? MemberBiography { get; set; }
        public string? Occupation { get; set; }
        public string? Business { get; set; }

        public string? ContactNumber { get; set; }
        public string? HomeAddress { get; set; }
        public string HomeCity { get; set; } = null!;

        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }

        public decimal TotalSavings { get; set; }
        public decimal TotalOpsContribution { get; set; }

        public UserSummaryDto? User { get; set; }
    }
}
