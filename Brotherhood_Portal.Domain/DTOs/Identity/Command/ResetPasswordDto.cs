using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.DTOs.Identity.Command
{ 
    public class ResetPasswordDto
    {
        public string UserId { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
