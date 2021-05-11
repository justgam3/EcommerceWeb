using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class ProductImage
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string Path { get; set; }

        public Product Product { get; set; }
    }
}
