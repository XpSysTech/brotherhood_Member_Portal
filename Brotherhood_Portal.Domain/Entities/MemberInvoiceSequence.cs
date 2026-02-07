using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Domain.Entities
{
    public class MemberInvoiceSequence
    {
        // Foreign Key
        public string MemberId { get; set; } = null!;
        public int CurrentNumber { get; set; }
        public int Year { get; set; }

        //Navigation Property
        public Member Member { get; set; } = null!;
    }
}
