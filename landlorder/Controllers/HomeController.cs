using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using landlorder.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;
using System.Data.Entity;

namespace landlorder.Controllers
{
    public class HomeController : Controller
    {
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
            return View();
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
            var relatedproperties = context.Database.SqlQuery<Property>("SearchReviews_StreetAddress_Related @lat, @lon, @pagenum",
                    new SqlParameter("@lat", array.latitude),
                    new SqlParameter("@lon", array.longitude),
                    //Set page at 1 since we are redirecting to the first page of results.
                    new SqlParameter("@pagenum", 1)).ToList();

            var result = new { property = property, relatedproperties = relatedproperties, Url = "/Home/Search" };
            return Json(result);
        }

    }
}