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
    public class Product_detailModel : PageModel
    {
        private readonly StoreAPI _api;

        public Product_detailModel()
        {
            _api = new StoreAPI();
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Products/" + id +"/GetProductDetails");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Product = JsonConvert.DeserializeObject<Product>(result);
            }

            if (Product == null)
            {
                return NotFound();
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                res = await client.GetAsync("api/Members/GetMemberByUsername/" + HttpContext.User.Identity.Name);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    var member = JsonConvert.DeserializeObject<Member>(result);
                    ViewData["MemberID"] = member.ID;
                }
            }

            return Page();
        }
    }
}
