using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EcommerceWebApi.Filter;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace EcommerceWebApp.Areas.User.Pages
{
    public class ProductModel : PageModel
    {
        private readonly StoreAPI _api;

        public ProductModel()
        {
            _api = new StoreAPI();
        }
        public IList<Product> Product { get; set; }

        public IList<Category> Category { get; set; }

        public PaginationFilter<Product> PaginationFilter { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {

            HttpClient client = _api.Initial();
            
            HttpResponseMessage res = await client.GetAsync("api/Products/GetProductImagesAndCategoriesByPagination?PageNumber=" + (pageNumber ?? 1) + "&PageSize=" + 8);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                PaginationFilter = JsonConvert.DeserializeObject<PaginationFilter<Product>>(result);
                Product = PaginationFilter.PagedData;
            }

            res = await client.GetAsync("api/Categories/GetActiveCategories");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Category = JsonConvert.DeserializeObject<IList<Category>>(result);
            }
        }
    }
}
