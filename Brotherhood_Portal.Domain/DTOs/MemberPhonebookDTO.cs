using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs
{
    public class MemberPhonebookDTO
    {
        public string Id { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string FirstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string? Occupation { get; set; }
        public string? Business { get; set; }
        public string ContactNumber { get; set; } = null!;
    }
}
