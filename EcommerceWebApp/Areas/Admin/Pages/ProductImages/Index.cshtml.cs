using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using System.Net.Http;
using EcommerceWebApp.Helpers;
using Newtonsoft.Json;

namespace EcommerceWebApp.Areas.Admin.Pages.ProductImages
{
    public class IndexModel : PageModel
    {
        private readonly StoreAPI _api;

        public IndexModel()
        {
            _api = new StoreAPI();
        }

        public IList<ProductImage> ProductImage { get;set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Action/GetProductImages/" + id);

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                ProductImage = JsonConvert.DeserializeObject<IList<ProductImage>>(result);
            }

            ViewData["ProductID"] = id;

            return Page();
        }
    }
}
