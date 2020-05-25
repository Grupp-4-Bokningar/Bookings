using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingService.Models
{
    public class BookingModell
    {
        public int Booking_Id { get; set; }
        public int Event_Id { get; set; }
        public string EventNamn { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public int User_Id { get; set; }
        public string User_Name { get; set; }
        public string User_Type { get; set; }
    }
}