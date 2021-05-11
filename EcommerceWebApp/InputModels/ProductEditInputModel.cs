using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.InputModels
{
    public class ProductEditInputModel : ProductInputModel
    {
        public ProductEditInputModel()
        {
            Variants = new List<Variant>();
            ProductImages = new List<ProductImage>();
        }

        public IFormFile[] Photos { get; set; }

        [Display(Name = "Categories")]
        public int[] SelectedCategory { get; set; }

        public IList<Variant> Variants { get; set; }

        [Display(Name = "Product's images")]
        public IList<ProductImage> ProductImages { get; set; }
    }
}
