using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.InputModels
{
    public class VariantInputModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int Stock { get; set; }
    }
}
