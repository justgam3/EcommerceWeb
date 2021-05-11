using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsAdmin { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Phone No.")]
            public string PhoneNo { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, bool isAdmin = false)
        {
            ReturnUrl = returnUrl;
            IsAdmin = isAdmin;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, bool isAdmin = false)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user, Input.Password);

                var member = new MemberHelper();

                bool memberResult = member.CreateMember
                    (
                    new Member {
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        PhoneNo = Input.PhoneNo,
                        Username = Input.Email,
                        Email = Input.Email,
                        CreatedAt = DateTime.Now,
                        IsActive = true,
                        Role = isAdmin ? "Administrator" : "Normal user"
                    });

                if (result.Succeeded && memberResult)
                {
                    if (isAdmin)
                    {
                        bool roleExists = await _roleManager.RoleExistsAsync("Administrator");
                        if (!roleExists)
                        {
                            var role = new IdentityRole();
                            role.Name = "Administrator";
                            await _roleManager.CreateAsync(role);
                        }
                        await _userManager.AddToRoleAsync(user, "Administrator");
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    string subject = "Confirm your email";
                    string htmlContent = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                    if (isAdmin)
                    {
                        subject = "Confirm " + Input.Email + " as an admin account";
                        htmlContent = $"Please confirm admin's account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                        await _emailSender.SendEmailAsync("boboken2@gmail.com", subject, htmlContent);
                    }
                    else
                    {
                        await _emailSender.SendEmailAsync(Input.Email, subject, htmlContent);
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl, isAdmin = isAdmin});
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task CreateRolesandUsers()
        {
            bool x = await _roleManager.RoleExistsAsync("Administrator");
            if (!x)
            {
                // first we create Admin rool   
                var role = new IdentityRole();
                role.Name = "Administrator";
                await _roleManager.CreateAsync(role);

                ////Here we create a Admin super user who will maintain the website                   

                //var user = new IdentityUser();
                //user.UserName = "admin123@gmail.com";
                //user.Email = "admin123@gmail.com";

                //string userPWD = "Admin_123";

                //IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

                ////Add default User to Role Admin    
                //if (chkUser.Succeeded)
                //{
                //    var result1 = await _userManager.AddToRoleAsync(user, role.Name);
                //}
            }
        }
    }
}
