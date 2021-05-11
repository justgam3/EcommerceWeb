using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class Order
    {
        public int ID { get; set; }

        public int MemberID { get; set; }

        public string PaymentID { get; set; }

        [Display(Name = "Order date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        [Display(Name = "Postcode")]
        public string PostCode { get; set; }

        public string FullAddress
        {
            get
            {
                return Country + ", " + State + ", " + PostCode;
            }
        }

        public string Status { get; set; }

        [Display(Name = "Status's Description")]
        public string StatusDesc { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        public Member Member { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }


    }
}
