using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Variant
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string Type { get; set; }

        public int Stock { get; set; }

        public Product Product { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
