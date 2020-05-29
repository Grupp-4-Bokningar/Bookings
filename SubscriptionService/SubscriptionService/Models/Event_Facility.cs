using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubscriptionService.Models
{
    public class Event_Facility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Place Place { get; set; }

    }
}