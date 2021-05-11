using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Product
    {

        public Product()
        {
            Variants = new List<Variant>();
            ProductImages = new List<ProductImage>();
        }

        public int ID { get; set; }

        [Display(Name = "Product's name")]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public IList<Variant> Variants { get; set; }

        public ICollection<MemberWishlist> MemberWishlists { get; set; }

        [Display(Name = "Product's categories")]
        public ICollection<ProductCategory> ProductCategories { get; set; }

        public ICollection<Message> Messages { get; set; }

        [Display(Name = "Product's images")]
        public IList<ProductImage> ProductImages { get; set; }
    }
}
