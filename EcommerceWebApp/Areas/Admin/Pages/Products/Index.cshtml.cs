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
using Microsoft.AspNetCore.Authorization;
using EcommerceWebApi.Filter;

namespace EcommerceWebApp.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly StoreAPI _api;

        public IndexModel()
        {
            _api = new StoreAPI();
        }
        public IList<Product> Product { get; set; }

        public PaginationFilter<Product> PaginationFilter { get; set; }

        public string CurrentSort { get; set; }

        public async Task OnGetAsync(int? pageNumber, string orderBy)
        {
         
            CurrentSort = orderBy;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Products/GetProductsByPagination?PageNumber=" + (pageNumber ?? 1) + "&PageSize=" + 5 + "&orderBy=" + orderBy);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                PaginationFilter = JsonConvert.DeserializeObject<PaginationFilter<Product>>(result);
                Product = PaginationFilter.PagedData;
            }
        }
    }
}
