namespace Adminprototype
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bookings
    {
        [Key]
        [Column(Order = 0)]
        public int Booking_Id { get; set; }

  
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Event_Id { get; set; }

   
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Event_Host_Id { get; set; }


        [Column(Order = 3)]
        public DateTime Event_Start_Date_Time { get; set; }

     
        [Column(Order = 4)]
        public double Booking_Price { get; set; }

 
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int User_Id { get; set; }


        [Column(Order = 6)]
        [StringLength(14)]
        public string User_Type { get; set; }
    }
}
