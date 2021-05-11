using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.Models;
using System.Net.Http;
using EcommerceWebApp.Helpers;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace EcommerceWebApp.Areas.Admin.Pages.ProductCategories
{
    public class CreateModel : PageModel
    {
        private readonly StoreAPI _api;

        public CreateModel()
        {
            _api = new StoreAPI();
        }

        public async Task<IActionResult> OnGetAsync(int? prod_id)
        {
            if(prod_id == null)
            {
                return NotFound();
            }
            ViewData["ID"] = prod_id;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Products");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var product = JsonConvert.DeserializeObject<IList<Product>>(result);
                ViewData["ProductID"] = new SelectList(product.Where(p => p.ID == prod_id), "ID", "ID");
            }
            res = await client.GetAsync("api/Categories");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var categories = JsonConvert.DeserializeObject<IList<Category>>(result);
                ViewData["CategoryID"] = new SelectList(categories.Where(c => c.ProductCategories.All(pc => pc.ProductID != prod_id) && c.IsActive), "ID", "Name");
            }
            
            return Page();
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HttpClient client = _api.Initial();
            await client.PostAsync("api/ProductCategories", new StringContent(
               JsonConvert.SerializeObject(ProductCategory), Encoding.UTF8, MediaTypeNames.Application.Json));

            return RedirectToPage("./Index", new { id = ProductCategory.ProductID });
        }
    }
}
