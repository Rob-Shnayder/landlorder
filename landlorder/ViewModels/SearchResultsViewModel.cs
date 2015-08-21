using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Device.Location;

namespace landlorder.ViewModels
{
    public partial class SearchResultsViewModel
    {
        public int propertyID { get; set; }       
        public string apartmentnum { get; set; }
        public int numofReviews { get; set; }
        public string formatted_address { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string type { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true)]
        public double averagerating { get; set; }     
        public GeoCoordinate geocoord { get; set; }
    }
}