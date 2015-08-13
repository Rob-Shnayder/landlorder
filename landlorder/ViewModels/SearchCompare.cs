using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Geocoding.Google;
namespace landlorder.ViewModels
{
    public class SearchCompare
    {
        public int propertyID { get; set; }
        public string streetaddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string route { get; set; }
        public string route_long { get; set; }
        public string city_long { get; set; }
        public string apartmentnum { get; set; }
        public string formatted_address { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public GoogleAddressComponent [] G1 { get; set; }
    }
}