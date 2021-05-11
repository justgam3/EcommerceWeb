using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }

        public int VariantID { get; set; }

        public int Quantity { get; set; }

        [Display(Name = "Product's name")]
        public string ProductName { get; set; }

        [Display(Name = "Product's price")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Product's image")]
        public string ProductImagePath { get; set; }

        [Display(Name = "Product's variant")]
        public string VariantType { get; set; }

        public Order Order { get; set; }

        public Variant Variant { get; set; }
    }
}
