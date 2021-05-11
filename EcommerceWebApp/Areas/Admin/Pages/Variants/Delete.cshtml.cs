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

namespace EcommerceWebApp.Areas.Admin.Pages.Variants
{
    public class DeleteModel : PageModel
    {
        private readonly StoreAPI _api;

        public DeleteModel()
        {
            _api = new StoreAPI();
        }

        [BindProperty]
        public Variant Variant { get; set; }

        public async Task<IActionResult> OnGetAsync(int? variant_id)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = _api.Initial();

            await client.DeleteAsync("api/Variants/" + Variant.ID);


            return RedirectToPage("./Index", new { id = Variant.ProductID });
        }
    }
}
