using System.Net.Http;
using System.Threading.Tasks;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace EcommerceWebApp.Pages
{
    public class Purchase_detailModel : PageModel
    {
        private readonly StoreAPI _api;

        public Purchase_detailModel()
        {
            _api = new StoreAPI();
        }

        [BindProperty]
        public Order Order { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/OrderDetails/" + id + "/GetOrderDetailById");
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
