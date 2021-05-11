using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using NLog;

namespace EcommerceWebApp.Areas.Admin.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly Logger _orderLogger = LogManager.GetLogger("orderLogger");
        private readonly ILogger<EditModel> _logger;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }

        [BindProperty]
        public string CurrentStatus { get; set; }

        [BindProperty]
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
                CurrentStatus = Order.Status;
            }

            if (Order == null)
            {
                return NotFound();
            }

            //res = await client.GetAsync("api/Members/");
            //if (res.IsSuccessStatusCode)
            //{
            //    var result = res.Content.ReadAsStringAsync().Result;
            //    var member = JsonConvert.DeserializeObject<IEnumerable<Member>>(result);
            //    ViewData["MemberID"] = new SelectList(member, "ID", "ID");
            //}
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UpdatedAt = DateTime.Now;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PutAsync("api/Orders/" + Order.ID, new StringContent(
               JsonConvert.SerializeObject(Order), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (res.IsSuccessStatusCode)
            {
                if (!CurrentStatus.Equals(Order.Status))
                {
                    _orderLogger.Info("Payment ID : {payment_id}, Order status has been changed from {current_status} >> {new_status}.", Order.PaymentID, CurrentStatus, Order.Status);
                }
                _logger.LogInformation("{user} has edited this order [ID: {order_id}].", HttpContext.User.Identity.Name, Order.ID);
                return RedirectToPage("./Index");
            }
            ViewData["Error"] = "Failed to edit record";

            return Page();
        }
    }
}
