namespace SubscriptionService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Subscriptions
    {
        [Key]
        public int Subscription_Id { get; set; }

        public int User_Id { get; set; }

        public int Event_Location_Id { get; set; }
    }
}
