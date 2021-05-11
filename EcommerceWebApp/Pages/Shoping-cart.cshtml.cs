using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using EcommerceWebApi.DTO;
using EcommerceWebApi.Models;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.InputModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;

namespace EcommerceWebApp.Areas.User.Pages
{
    [Authorize]
    public class Shoping_cartModel : PageModel
    {
        private readonly StoreAPI _api;
        private readonly ILogger<Shoping_cartModel> _logger;

        public Shoping_cartModel(ILogger<Shoping_cartModel> logger)
        {
            _logger = logger;
            _api = new StoreAPI();
        }

        [BindProperty]
        public UserCartDetailDTO CartDetailDTO { get; set; }

        [BindProperty]
        public UserCartInputModel CartInputModel { get; set; }

        private const string ERROR_MSG = "Failed to make payment. Please try again.";

        private const string OUT_OF_STOCK_MSG = "Insufficient stock. Please try again.";

        public async Task<IActionResult> OnGetAsync()
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Members/GetMemberByUsername/" + HttpContext.User.Identity.Name);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                var member = JsonConvert.DeserializeObject<Member>(result);
                ViewData["MemberID"] = member.ID;

                res = await client.GetAsync("api/CartDetails/" + member.ID + "/GetCartDetailById");
                if (res.IsSuccessStatusCode)
                {
                    result = res.Content.ReadAsStringAsync().Result;
                    CartDetailDTO = JsonConvert.DeserializeObject<UserCartDetailDTO>(result);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostChargeAsync(string stripeEmail, string stripeToken)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/CartDetails/" + CartDetailDTO.MemberID + "/GetCartDetailById");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                CartDetailDTO = JsonConvert.DeserializeObject<UserCartDetailDTO>(result);
            }

            if (!res.IsSuccessStatusCode || !ModelState.IsValid)
            {
                ViewData["Error"] = ERROR_MSG;
                return Page();
            }

            try
            {
                decimal totalPrice = CartDetailDTO.TotalPrice;

                var customerOptions = new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Source = stripeToken,
                };
                var customer = new CustomerService().Create(customerOptions);

                //var paymentMethodOptions = new PaymentMethodCreateOptions
                //{
                //    Type = "card",
                //    Card = new PaymentMethodCardOptions
                //    {
                //        Number = "4242424242424242",
                //        ExpMonth = 4,
                //        ExpYear = 2022,
                //        Cvc = "314",
                //    },
                //};
                //var paymentMethod = new PaymentMethodService().Create(paymentMethodOptions);

                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)(totalPrice * 100),
                    Currency = "usd",
                    Description = "Test Payment",
                    Customer = customer.Id,
                    ReceiptEmail = stripeEmail,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Country", CartInputModel.Country },
                        { "State", CartInputModel.State },
                        { "Postcode", CartInputModel.Postcode },
                    },
                    PaymentMethod = "pm_card_visa",
                };

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = service.Create(paymentIntentOptions);

                var order = new EcommerceWebApi.Models.Order();

                order.MemberID = CartDetailDTO.MemberID;
                order.PaymentID = paymentIntent.Id;
                order.OrderDate = DateTime.Now;
                order.Country = CartInputModel.Country;
                order.State = CartInputModel.State;
                order.PostCode = CartInputModel.Postcode;
                order.Status = "To Ship";
                order.StatusDesc = "Waiting to be shipped out.";
                order.TotalPrice = totalPrice;

                bool stockSufficient = true;
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach (var item in CartDetailDTO.CartDetails)
                {
                    orderDetails.Add(new OrderDetail
                    {
                        Quantity = item.Quantity,
                        VariantID = item.VariantID,
                        ProductName = item.Variant.Product.ProductName,
                        ProductPrice = item.Variant.Product.Price,
                        ProductImagePath = item.Variant.Product.ProductImages.FirstOrDefault().Path,
                        VariantType = item.Variant.Type
                    });

                    HttpResponseMessage variant_res = await client.PutAsync("api/Action/UpdateVariantQuantity/" + item.VariantID + "/" + item.Quantity, new StringContent(
                           JsonConvert.SerializeObject(order, new JsonSerializerSettings
                           {
                               ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           }), Encoding.UTF8, MediaTypeNames.Application.Json));
                    if (variant_res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var result = variant_res.Content.ReadAsStringAsync().Result;
                        if (result.Equals("Out of stock"))
                        {
                            item.Variant.Stock = 0;
                            stockSufficient = false;
                            //CartDetailDTO.CartDetails.Remove(item);
                            //CartDetailDTO.TotalPrice = CartDetailDTO.CartDetails.Sum(cd => cd.Quantity * cd.Variant.Product.Price);
                            //ViewData["Error"] = OUT_OF_STOCK_MSG;
                        }
                        
                    }
                }

                if (!stockSufficient)
                {
                    service.Cancel(paymentIntent.Id);
                    return Page();
                }

                order.OrderDetails = orderDetails;

                HttpResponseMessage insert_res = await client.PostAsync("api/Orders", new StringContent(
                   JsonConvert.SerializeObject(order, new JsonSerializerSettings
                   {
                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                   }), Encoding.UTF8, MediaTypeNames.Application.Json));

                HttpResponseMessage delete_res = await client.DeleteAsync("api/Action/DeleteCart/" + CartDetailDTO.MemberID);
                if (insert_res.IsSuccessStatusCode && delete_res.IsSuccessStatusCode)
                {
                    paymentIntent = service.Confirm(paymentIntent.Id);   
                }

                if (paymentIntent.Status == "succeeded")
                {
                    _logger.LogInformation("Order is successfully made by {user_email} with PaymentID >> {payment_id}", stripeEmail, paymentIntent.Id);
                    return RedirectToPage("My-purchase");
                }
                else
                {
                    service.Cancel(paymentIntent.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.ToString());
            }

            ViewData["Error"] = ERROR_MSG;
            return Page();
        }

        //private static void Refund(PaymentIntent intent)
        //{
        //    var service = new PaymentIntentService();
        //    service.Cancel(intent.Id);
        //    //var refunds = new RefundService();
        //    //var refundOptions = new RefundCreateOptions
        //    //{
        //    //    PaymentIntent = intent.Id,
        //    //    Reason = reason,
        //    //};
        //    //refunds.Create(refundOptions);
        //}
    }
}
