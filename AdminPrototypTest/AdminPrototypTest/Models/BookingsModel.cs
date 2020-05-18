using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminPrototypTest.Models
{
    public class BookingsModel
    {
        public int Booking_Id {get; set;}
        public int Event_Id { get; set; }
        public int Event_Host_Id { get; set; }
        public DateTime Event_Start_Date_Time { get; set; }

        public float Booking_Price { get; set; }
        public int User_Id { get; set; }
        public string User_Type { get; set; }
    }
}