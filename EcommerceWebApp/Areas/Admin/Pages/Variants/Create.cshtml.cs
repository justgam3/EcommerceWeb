using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.Models;
using EcommerceWebApp.InputModels;
using EcommerceWebApp.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Mime;

namespace EcommerceWebApp.Areas.Admin.Pages.Variants
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
            if (prod_id == null)
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
            return Page();
        }

        public Variant Variant { get; set; }

        [BindProperty]
        public VariantInputModel InputModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var variant = new Variant
            {
                ProductID = InputModel.ProductID,
                Type = InputModel.Type,
                Stock = InputModel.Stock
            };

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("api/Variants", new StringContent(
               JsonConvert.SerializeObject(variant), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (!res.IsSuccessStatusCode)
            {
                ViewData["Error"] = "Failed to insert record";
                await this.OnGetAsync(InputModel.ProductID);
                return Page();
            }
            return RedirectToPage("./Index", new { id = InputModel.ProductID });

        }
    }
}
