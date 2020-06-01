using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAdminPage.Models
{
    public class AdminDataModell
    {
        public int Booking_Id { get; set; }
        public int Event_Id { get; set; }
        public int User_Id { get; set; }
        public string Event_Name { get; set; }
        public DateTime? Event_Start_Datetime { get; set; }
        public DateTime? Event_End_Datetime { get; set; }
        public string User_Name { get; set; }
        public string User_Type { get; set; }
    }
}