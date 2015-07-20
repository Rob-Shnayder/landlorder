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

        public ActionResult Search()
        {
            if (TempData["doc"] != null)
            {
                
                return View();
            }
            return View();        
        }

        [HttpPost]
        public JsonResult GetLocationData(StreetAddressModel array)
        {            
            using (var context = new landlorderEntities2())
            {                
                var property = context.Database.SqlQuery<Property>("SearchReviews_StreetAddress @streetaddress, @route, @city,@state,@postal_code",
                    new SqlParameter("@streetaddress", array.street_number),
                    new SqlParameter("@route", array.route),
                    new SqlParameter("@city", array.city),
                    new SqlParameter("@state", array.state),
                    new SqlParameter("@postal_code", array.postal_code)).ToList();

                TempData["doc"] = property;                
            }
            return Json(new { Url = "Home/Search"});
        }

    }
}