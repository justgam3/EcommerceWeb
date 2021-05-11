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
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly StoreAPI _api;

        public EditModel(ILogger<EditModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Categories/" + id);

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Category = JsonConvert.DeserializeObject<Category>(result);
            }

            if (Category == null)
            {
                return NotFound();
            }
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

            Category.UpdatedAt = DateTime.Now;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PutAsync("api/Categories/" + Category.ID, new StringContent(
               JsonConvert.SerializeObject(Category), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("{user} has edited this category [ID: {category_id}].", HttpContext.User.Identity.Name, Category.ID);
                return RedirectToPage("./Index");
            }
            ViewData["Error"] = "Failed to edit record";
            return Page();
        }
    }
}
