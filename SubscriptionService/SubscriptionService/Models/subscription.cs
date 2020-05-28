namespace SubscriptionService.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("subscription")]
    public partial class subscription
    {
        [Key]
        public int subscription_Id { get; set; }

        public int user_Id { get; set; }

        public int event_location_Id { get; set; }
    }
}
