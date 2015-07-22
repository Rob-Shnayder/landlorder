using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace landlorder.Models
{
    public class StreetAddressModel
    {
        public string street_number { get; set; }
        public string route { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string formatted_address { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string type { get; set; }
        public string vicinity { get; set; }
    }

    public class AreaModel
    {
        public string postal_code { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }
}