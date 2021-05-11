using EcommerceWebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.DTO
{
    public class ProductDTO
    {
        public ProductDTO()
        {
            Variants = new List<Variant>();
        }

        public int ID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public IList<Variant> Variants { get; set; }

        [Display(Name = "Categories")]
        public ICollection<ProductCategory> ProductCategories { get; set; }

        [Display(Name = "Images")]
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}
