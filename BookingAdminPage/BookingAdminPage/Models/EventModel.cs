using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BookingService.Models
{
    public class EventModel
    {
        public int Event_Id { get; set; }
        public string Event_Name { get; set; }
        public string Event_Description { get; set; }
        public bool Event_Active { get; set; }
        public int Event_Arranger_Id { get; set; }
        public bool Event_Seeking_Volunteers{ get; set; }
        public Facility Event_Facility { get; set; }
        public Category Event_Category { get; set; }
        public string Event_Imagelink { get; set; }
        public float Event_Ticket_Price { get; set; }
        public Organizer Event_Organizer { get; set; }
        public DateTime Event_Start_Datetime { get; set; }
        public DateTime Event_End_Datetime { get; set; }
        public DateTime Event_Create_Datetime { get; set; }
    }
}