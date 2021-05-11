using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.InputModels
{
    public class ProductCreateInputModel : ProductInputModel
    {

        public ProductCreateInputModel()
        {
            Variants = new List<Variant>();
            ProductImages = new List<ProductImage>();
        }

        [Required]
        public IFormFile[] Photos { get; set; }

        [Required]
        public int[] SelectedCategory { get; set; }

        public IList<Variant> Variants { get; set; }

        public IList<ProductImage> ProductImages { get; set; }

    }
}
