using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        [BindProperty(Name = "source", SupportsGet = true)]
        public string ExceptionSource { get; set; }

        [BindProperty(Name = "message", SupportsGet = true)]
        public string ExceptionMessage { get; set; }

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //_logger.LogError(ExceptionHandler.ToString());
            //RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            //var exceptionHandlerPathFeature =
            //HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            //{
            //    ExceptionMessage = "File error thrown";
            //    _logger.LogError(ExceptionMessage);
            //}
            //if (exceptionHandlerPathFeature?.Path == "/index")
            //{
            //    ExceptionMessage += " from home page";
            //}
        }
    }
}
