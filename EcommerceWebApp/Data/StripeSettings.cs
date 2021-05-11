using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.Data
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }

        public string PublishableKey { get; set; }
    }
}
