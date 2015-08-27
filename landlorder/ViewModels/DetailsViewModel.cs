using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using landlorder.Models;
using System.ComponentModel.DataAnnotations;
using PagedList;
using PagedList.Mvc;

namespace landlorder.ViewModels
{
    public class DetailsViewModel
    {
        public int propertyID { get; set; }        
        public string formatted_address { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public int numOfReviews { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true)]
        public double overallRating { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true)]
        public double repairRating { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true)]
        public double communicationRating { get; set; }   

        public virtual IOrderedEnumerable<Review> Reviews { get; set; }

        public virtual AspNetUser users { get; set; }

        public StaticPagedList<Review> PagedReviews { get; set; }
        
    }
}