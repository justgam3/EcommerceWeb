using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.InputModels
{
    public class ProductImageInputModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public IFormFile[] Photo { get; set; }

    }
}
