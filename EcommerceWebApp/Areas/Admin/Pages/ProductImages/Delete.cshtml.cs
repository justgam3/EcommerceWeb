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

namespace EcommerceWebApp.Areas.Admin.Pages.ProductImages
{
    public class DeleteModel : PageModel
    {
        private readonly StoreAPI _api;

        public DeleteModel()
        {
            _api = new StoreAPI();
        }

        [BindProperty]
        public ProductImage ProductImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Action/GetProductImage/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                ProductImage = JsonConvert.DeserializeObject<ProductImage>(result);
            }

            if (ProductImage == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = _api.Initial();

            await client.DeleteAsync("api/ProductImages/" + ProductImage.ID);


            return RedirectToPage("./Index", new { id = ProductImage.ProductID });
        }
    }
}
