using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EcommerceWebApi.DTO;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace EcommerceWebApp.Areas.User.Pages
{
    public class My_purchaseModel : PageModel
    {
        private readonly StoreAPI _api;

        public My_purchaseModel()
        {
            _api = new StoreAPI();
            Order = new List<Order>();
        }

        public IList<Order> Order { get; set; }

        public async Task OnGetAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("api/Orders/" + HttpContext.User.Identity.Name +"/GetOrderByUsername");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    Order = JsonConvert.DeserializeObject<IList<Order>>(result);
                }
            }
        }
    }
}
