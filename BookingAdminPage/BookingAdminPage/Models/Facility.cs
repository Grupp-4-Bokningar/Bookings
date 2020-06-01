using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facility.Models
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Place place { get; set; }
    }
}