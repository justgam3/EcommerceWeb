using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.DTO
{
    public class UserCartDetailDTO
    {
        public int MemberID { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
