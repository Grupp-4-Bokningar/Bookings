using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookingAdminPage.Models
{
    public class Category
    {
        public int Category_Id { get; set; }

        [StringLength(50)]

        public string Category_Name { get; set; }
    }
}