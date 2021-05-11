using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Member
    {
        public int ID { get; set; }

        public string Username { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone no.")]
        public string PhoneNo { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Last login")]
        public DateTime? LastLogin { get; set; }

        public string Role { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; }

        public ICollection<MemberWishlist> MemberWishlists { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
