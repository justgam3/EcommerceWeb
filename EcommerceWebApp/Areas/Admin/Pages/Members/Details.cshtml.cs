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

namespace EcommerceWebApp.Areas.Admin.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly StoreAPI _api;

        public DetailsModel()
        {
            _api = new StoreAPI();
        }

        public Member Member { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Members/" + id);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Member = JsonConvert.DeserializeObject<Member>(result);
            }

            if (Member == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
