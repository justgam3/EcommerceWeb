using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using System.Net.Http;
using Newtonsoft.Json;
using EcommerceWebApp.Helpers;

namespace EcommerceWebApp.Areas.Admin.Pages.ProductCategories
{
    public class DeleteModel : PageModel
    {
        private readonly StoreAPI _api;

        public DeleteModel()
        {
            _api = new StoreAPI();
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? prod_id, int? cat_id)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Action/GetProductCategories/" + prod_id + "/" + cat_id);

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                ProductCategory = JsonConvert.DeserializeObject<ProductCategory>(result);
            }

            if (ProductCategory == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            HttpClient client = _api.Initial();
            
            await client.DeleteAsync("api/Action/DeleteProductCategory/" + ProductCategory.ProductID + "/" + ProductCategory.CategoryID);

            return RedirectToPage("./Index", new { id = ProductCategory.ProductID });
        }
    }
}
