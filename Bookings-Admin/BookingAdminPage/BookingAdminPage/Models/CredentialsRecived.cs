using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAdminPage.Models
{
    public class CredentialsRecived
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string permission { get; set; }

    }
}