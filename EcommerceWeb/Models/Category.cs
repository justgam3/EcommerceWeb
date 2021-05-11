using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Category
    {
        public int ID { get; set; }

        public string Name { get; set; }

        [Display (Name="Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
