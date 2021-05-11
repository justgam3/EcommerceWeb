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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Members
{
    public class EditModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<EditModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(UserManager<IdentityUser> userManager, ILogger<EditModel> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _api = new StoreAPI();
            
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var member = await _userManager.FindByEmailAsync(Member.Email);
            if (!Member.IsActive)
            {
                member.LockoutEnd = DateTimeOffset.MaxValue;
                member.LockoutEnabled = true;
            }
            else
            {
                
                member.LockoutEnd = null;
                member.LockoutEnabled = false;

            }
            await _userManager.UpdateAsync(member);

            Member.UpdatedAt = DateTime.Now;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PutAsync("api/Members/" + Member.ID, new StringContent(
               JsonConvert.SerializeObject(Member), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (res.IsSuccessStatusCode)
            {
                _logger.LogInformation("{user} has edited this member [ID: {member_id}].", HttpContext.User.Identity.Name, Member.ID);
                return RedirectToPage("./Index");

            }
            return RedirectToPage("./Index");
        }
    }
}
