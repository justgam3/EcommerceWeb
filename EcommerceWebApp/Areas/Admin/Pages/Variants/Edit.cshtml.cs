using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;

namespace EcommerceWebApp.Areas.Admin.Pages.Variants
{
    public class EditModel : PageModel
    {
        private readonly StoreAPI _api;

        public EditModel()
        {
            _api = new StoreAPI();
        }

        [BindProperty]
        public Variant Variant { get; set; }

        public async Task<IActionResult> OnGetAsync(int? variant_id, int? prod_id)
        {
            if (variant_id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Action/GetProductVariant/" + variant_id);

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Variant = JsonConvert.DeserializeObject<Variant>(result);
            }

            if (Variant == null)
            {
                return NotFound();
            }
            res = await client.GetAsync("api/Products");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var product = JsonConvert.DeserializeObject<IList<Product>>(result);
                ViewData["ProductID"] = new SelectList(product.Where(p => p.ID == prod_id), "ID", "ID");
            }
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PutAsync("api/Variants/" + Variant.ID, new StringContent(
               JsonConvert.SerializeObject(Variant), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (!res.IsSuccessStatusCode)
            {
                ViewData["Error"] = "Failed to insert record";
                await this.OnGetAsync(Variant.ID, Variant.ProductID);
                return Page();
            }

            return RedirectToPage("./Index", new { id = Variant.ProductID });
        }
    }
}
