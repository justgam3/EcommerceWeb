using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using EcommerceWebApp.InputModels;
using EcommerceWebApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;

namespace EcommerceWebApp.Areas.Admin.Pages.ProductImages
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


        public ProductImage ProductImage { get; set; }

        [BindProperty]
        public ProductImageInputModel Input { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<ProductImage> listImages = ProductImageHelper.UploadProductImage(Input);

            HttpClient client = _api.Initial();

            foreach (var item in listImages)
            {
                await client.PostAsync("api/ProductImages", new StringContent(
                    JsonConvert.SerializeObject(item), Encoding.UTF8, MediaTypeNames.Application.Json));
            }


            return RedirectToPage("./Index", new { id = Input.ProductID });
        }
    }
}
