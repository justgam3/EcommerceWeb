using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.DTO;

namespace EcommerceWebApp.Areas.Admin.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly StoreAPI _api;

        public DetailsModel()
        {
            _api = new StoreAPI();
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Products/" + id + "/GetAdminProductDetails");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Product = JsonConvert.DeserializeObject<Product>(result);
            }

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
