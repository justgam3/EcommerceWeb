using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Mime;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Category.CreatedAt = DateTime.Now;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("api/Categories", new StringContent(
               JsonConvert.SerializeObject(Category), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                Category tempCategory = JsonConvert.DeserializeObject<Category>(result);
                _logger.LogInformation("{user} has created a new category [ID: {category_id}].", HttpContext.User.Identity.Name, tempCategory.ID);
                return RedirectToPage("./Index");
            }

            ViewData["Error"] = "Failed to insert record";
            return Page();
        }
    }
}
