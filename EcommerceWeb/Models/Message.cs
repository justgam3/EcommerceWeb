using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Message
    {
        public int ProductID { get; set; }

        public int MemberID { get; set; }

        public string Review { get; set; }

        public short Rating { get; set; }

        public Member Member { get; set; }

        public Product Product { get; set; }
    }
}
