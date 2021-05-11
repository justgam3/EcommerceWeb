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
using EcommerceWebApi.Filter;
using Microsoft.Extensions.Logging;

namespace EcommerceWebApp.Areas.Admin.Pages.Members
{
    public class IndexModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _api = new StoreAPI();
            _logger = logger;
        }

        public IList<Member> Member { get;set; }

        public PaginationFilter<Member> PaginationFilter { get; set; }

        public string CurrentSort { get; set; }

        public async Task OnGetAsync(int? pageNumber, string orderBy)
        {

            CurrentSort = orderBy;

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Members/GetMembersByPagination?PageNumber=" + (pageNumber ?? 1) + "&PageSize=" + 5 + "&orderBy=" + orderBy);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                PaginationFilter = JsonConvert.DeserializeObject<PaginationFilter<Member>>(result);
                Member = PaginationFilter.PagedData;
            }
        }
    }
}
