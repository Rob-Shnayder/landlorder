using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace landlorder.Models
{
    public class LocationInfo
    {
        public string street_number { get; set; }
        public string route { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string postal_code_suffix { get; set; }
    }
}