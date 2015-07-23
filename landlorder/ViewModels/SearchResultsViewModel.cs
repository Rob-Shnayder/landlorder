using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace landlorder.ViewModels
{
    public partial class SearchResultsViewModel
    {
        public int propertyID { get; set; }
        public string streetaddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string route { get; set; }
        public string apartmentnum { get; set; }
        public int numofReviews { get; set; }
    }
}