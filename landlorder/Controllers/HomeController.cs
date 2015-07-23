﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using landlorder.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.Data.Entity;
using landlorder.ViewModels;

namespace landlorder.Controllers
{
    public class HomeController : Controller
    {
        private landlorderEntities2 db = new landlorderEntities2();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        //GET
        public ActionResult Search(string locationinput)
        {
            //When pulling apartments, do a group by on rows that do not have a 
            //APT number entry. For ones that match each other group those together.

            List<SearchResultsViewModel> query1 =
                (from p in db.Properties
                select new SearchResultsViewModel
                {
                    propertyID = p.propertyID,
                    streetaddress = p.streetaddress,
                    city = p.city,
                    zip = p.zip,
                    state = p.state,
                    country = p.country,
                    numofReviews = p.Reviews.Count(),
                    route = p.route,
                    //apartmentnum = ""
                    apartmentnum = db.Properties.Include(r => r.Reviews.Select(b => b.apartmentnum).Where(q => r.propertyID == p.propertyID)).ToString()
                    //db.Properties.Include(r => r.Reviews.Select(b=>b.apartmentnum).Where(q=> r.propertyID == p.propertyID ))
                }).ToList();


            return View(query1);
        }

        

        [HttpPost]
        public JsonResult GetLocationData(StreetAddressModel array)
        {
            if (array.city == null)
            {
                array.city = "";
            }
            if (array.postal_code == null)
            {
                array.postal_code = "";
            }

            var context = new landlorderEntities2();                  
                //Look for exact address match
            var property = context.Database.SqlQuery<Property>("SearchReviews_StreetAddress @streetaddress, @route, @city,@state,@postal_code",
                    new SqlParameter("@streetaddress", array.street_number),
                    new SqlParameter("@route", array.route),
                    new SqlParameter("@city", array.city),
                    new SqlParameter("@state", array.state),
                    new SqlParameter("@postal_code", array.postal_code)).ToList();
                                
                //Get related matches
            var relatedproperties = context.Database.SqlQuery<Property>("SearchReviews_StreetAddress_Related @lat, @lon, @vicinity,@pagenum",
                    new SqlParameter("@lat", array.latitude),
                    new SqlParameter("@lon", array.longitude),
                    new SqlParameter("@vicinity", array.vicinity),
                    new SqlParameter("@pagenum", 1)).ToList();

            var result = new { property = property, relatedproperties = relatedproperties, Url = "/Home/Search" };
            return Json(result);
        }


    }
}