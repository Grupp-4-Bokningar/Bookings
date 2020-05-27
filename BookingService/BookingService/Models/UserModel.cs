using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingService.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}