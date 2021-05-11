using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceWebApi.Models;
using System.Net.Http;
using EcommerceWebApp.Helpers;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using EcommerceWebApp.InputModels;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            var product = new Product();
            HttpResponseMessage res = await client.GetAsync("api/Products/" + id + "/GetAdminProductDetails");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(result);
            }


            res = await client.GetAsync("api/Categories/GetActiveCategories");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                IEnumerable<Category> activeCategories = JsonConvert.DeserializeObject<IEnumerable<Category>>(result);

                object[] selectedValues = new object[product.ProductCategories.Count];
                ushort i = 0;
                foreach (var item in product.ProductCategories)
                {
                    selectedValues[i++] = item.Category.ID;
                }
                SelectedCategories = new MultiSelectList(activeCategories, "ID", "Name", selectedValues);

            }

            if (product == null)
            {
                return NotFound();
            }

            InputModel = new ProductEditInputModel();
            InputModel.ID = product.ID;
            InputModel.IsActive = product.IsActive;
            InputModel.Price = product.Price;
            InputModel.CreatedAt = product.CreatedAt;
            InputModel.Description = product.Description;
            InputModel.ProductName = product.ProductName;
            InputModel.ProductImages = product.ProductImages;
            InputModel.Variants = product.Variants;

            return Page();
        }

        [BindProperty]
        public ProductEditInputModel InputModel { get; set; }

        public MultiSelectList SelectedCategories { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var product = new Product();

            List<ProductCategory> categories = new List<ProductCategory>();
            for (int i = 0; i < InputModel.SelectedCategory.Length; i++)
            {
                categories.Add(new ProductCategory
                {
                    CategoryID = InputModel.SelectedCategory[i],
                });
            }

            product.ProductCategories = categories;

            if (InputModel.Photos != null)
            {
                foreach (IFormFile photo in InputModel.Photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photo.FileName);
                    var stream = new FileStream(path, FileMode.Create);

                    try
                    {
                        photo.CopyTo(stream);
                        InputModel.ProductImages.Add(new ProductImage
                        {
                            Path = photo.FileName,
                        });
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }

            product.ID = InputModel.ID;
            product.IsActive = InputModel.IsActive;
            product.Price = InputModel.Price;
            product.CreatedAt = InputModel.CreatedAt;
            product.Description = InputModel.Description;
            product.ProductName = InputModel.ProductName;
            product.ProductImages = InputModel.ProductImages;
            product.Variants = InputModel.Variants;
            product.UpdatedAt = DateTime.Now;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.PutAsync("api/Products/" + product.ID + "/PutAdminProduct", new StringContent(
               JsonConvert.SerializeObject(product, new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }), Encoding.UTF8, MediaTypeNames.Application.Json)).Result;

            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("{user} has edited this product [ID: {product_id}].", HttpContext.User.Identity.Name, product.ID);
                return RedirectToPage("./Index");

            }
            ViewData["ErrorMsg"] = "Failed to insert this record.";
            return Page();
        }
    }
}
