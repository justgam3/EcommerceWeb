using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebApp.Helpers
{
    public class MemberHelper
    {
        private readonly StoreAPI _api;

        public MemberHelper()
        {
            _api = new StoreAPI();
        }

        //public async Task<Member> GetMemberAsync(string email)
        //{
        //    HttpClient client = _api.Initial();
        //    HttpResponseMessage res = await client.GetAsync("api/Members/GetMemberByEmail/" + email);
        //    if (res.IsSuccessStatusCode)
        //    {
        //        var result = res.Content.ReadAsStringAsync().Result;
        //        var member = JsonConvert.DeserializeObject<Member>(result);
        //        return member;
        //    }

        //    return null;
        //}

        public bool CreateMember(Member member)
        {

            HttpClient client = _api.Initial();
            HttpResponseMessage res = client.PostAsync("api/Members", new StringContent(
               JsonConvert.SerializeObject(member, new JsonSerializerSettings
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
               }), Encoding.UTF8, MediaTypeNames.Application.Json)).Result;

            if (!res.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateLastLogin(string email)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Members/GetMemberByEmail/" + email);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var member = JsonConvert.DeserializeObject<Member>(result);
                member.LastLogin = DateTime.Now;
                res = await client.PutAsync("api/Members/" + member.ID, new StringContent(
                        JsonConvert.SerializeObject(member), Encoding.UTF8, MediaTypeNames.Application.Json));
                if (res.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> GetMemberActiveStatus(string email)
        {

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Members/GetMemberStatusByEmail/" + email);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var activeResult = JsonConvert.DeserializeObject<bool>(result);
                if (!activeResult)
                    return false;
            }

            return true;
        }
    }
}
