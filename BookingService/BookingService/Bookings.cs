namespace BookingService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bookings
    {
        [Key]
        public int Booking_Id { get; set; }

        public int Event_Id { get; set; }

        public int User_Id { get; set; }

        [Required]
        [StringLength(14)]
        public string User_Type { get; set; }
    }
}
