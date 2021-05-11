using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using EcommerceWebApp.Helpers;
using EcommerceWebApi.Filter;
using System.Net.Http;
using Newtonsoft.Json;

namespace EcommerceWebApp.Areas.Admin.Pages.Orders
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly StoreAPI _api;

        public IndexModel()
        {
            _api = new StoreAPI();
        }

        public IList<Order> Order { get; set; }

        public PaginationFilter<Order> PaginationFilter { get; set; }

        public string CurrentSort { get; set; }

        public async Task OnGetAsync(int? pageNumber, string orderBy)
        {
            CurrentSort = orderBy;
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Orders/GetOrdersByPagination?PageNumber=" + (pageNumber ?? 1) + "&PageSize=" + 5 + "&orderBy=" + orderBy);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                PaginationFilter = JsonConvert.DeserializeObject<PaginationFilter<Order>>(result);
                Order = PaginationFilter.PagedData;
            }
        }
    }
}
