using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.Entities
{
    public class JwtSettings
    {
        public string TokenKey { get; set; } = null!;
        public int ExpiryMinutes { get; set; } = 15;
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
