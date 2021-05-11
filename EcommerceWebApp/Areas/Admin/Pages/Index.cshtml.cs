using EcommerceWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EcommerceWebApp.Pages
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly StoreAPI _api;

        public IndexModel()
        {
            _api = new StoreAPI();
        }

        public void OnGet()
        {
            
            RetrieveTotal("api/Products/GetProductCount", "Product");
            RetrieveTotal("api/Orders/GetOrderCount", "Order");
            RetrieveTotal("api/Categories/GetCategoryCount", "Category");
            RetrieveTotal("api/Members/GetMemberCount", "Member");
        }

        private void RetrieveTotal(string apiUrl, string name)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.GetAsync(apiUrl).Result;

            if (res.IsSuccessStatusCode)
            {
                ViewData[name + "Count"] = res.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
