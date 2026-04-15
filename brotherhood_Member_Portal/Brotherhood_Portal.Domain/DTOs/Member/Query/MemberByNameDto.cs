using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Member.Query
{
    public class MemberByNameDTO
    {
        public string Id { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
