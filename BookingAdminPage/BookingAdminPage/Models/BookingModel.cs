using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAdminPage.Models
{
    public class BookingModel
    {
        public int Booking_Id { get; set; }
        public int Event_Id { get; set; }
        public int User_Id { get; set; }

        public string User_Type { get; set; }
    }
}