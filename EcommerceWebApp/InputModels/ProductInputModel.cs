using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.InputModels
{
    public class ProductInputModel
    {


        public int ID { get; set; }

        [Required]
        [Display(Name = "Product's name")]
        public string ProductName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

    }
}
