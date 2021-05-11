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

namespace EcommerceWebApp.Areas.Admin.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly StoreAPI _api;

        public DetailsModel()
        {
            _api = new StoreAPI();
        }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Orders/" + id + "/GetAdminOrderDetails");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Order = JsonConvert.DeserializeObject<Order>(result);
            }

            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
