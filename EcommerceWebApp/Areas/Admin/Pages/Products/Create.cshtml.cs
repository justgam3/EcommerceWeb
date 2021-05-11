using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using EcommerceWebApp.InputModels;
using Microsoft.AspNetCore.Authorization;
using EcommerceWebApi.DTO;
using System.IO;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }


        public async Task<IActionResult> OnGetAsync()
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Categories/GetActiveCategories");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                IEnumerable<Category> categoryList = JsonConvert.DeserializeObject<IEnumerable<Category>>(result);
                ViewData["Category"] = new SelectList(categoryList, "ID", "Name");
            }
            return Page();
        }

        [BindProperty]
        public ProductCreateInputModel InputModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var product = new Product 
            {
                ProductName = InputModel.ProductName,
                Price = InputModel.Price,
                IsActive = InputModel.IsActive,
                CreatedAt = DateTime.Now,
                Description = InputModel.Description,
                Variants = InputModel.Variants
            };
            List<ProductCategory> categories = new List<ProductCategory>();
            for (int i = 0; i < InputModel.SelectedCategory.Length; i++)
            {
                categories.Add(new ProductCategory
                {
                    CategoryID = InputModel.SelectedCategory[i],
                });
            }

            product.ProductCategories = categories;

            foreach (IFormFile photo in InputModel.Photos)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photo.FileName);
                var stream = new FileStream(path, FileMode.Create);

                try
                {
                    photo.CopyTo(stream);
                    product.ProductImages.Add(new ProductImage
                    {
                        Path = photo.FileName,
                    });
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    stream.Close();
                }
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("api/Products", new StringContent(
               JsonConvert.SerializeObject(product, new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Product tempProduct = JsonConvert.DeserializeObject<Product>(result);
                _logger.LogInformation("{user} has created a new product [ID: {product_id}].", HttpContext.User.Identity.Name, tempProduct.ID);
                return RedirectToPage("./Index");
            }
            
            ViewData["ErrorMsg"] = "Failed to insert this record.";
            return Page();
        }
    }
}
