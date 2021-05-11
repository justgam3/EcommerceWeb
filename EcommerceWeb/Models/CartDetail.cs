using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class CartDetail
    {
        public int MemberID { get; set; }

        public int VariantID { get; set; }

        public int Quantity { get; set; }

        public Member Member { get; set; }

        public Variant Variant { get; set; }
    }
}
