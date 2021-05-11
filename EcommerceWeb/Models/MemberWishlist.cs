using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class MemberWishlist
    {
        public int MemberID { get; set; }

        public int ProductID { get; set; }

        public Member Member { get; set; }

        public Product Product { get; set; }


    }
}
