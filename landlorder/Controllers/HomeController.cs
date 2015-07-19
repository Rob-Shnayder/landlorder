using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using landlorder.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.Sql;

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

        public ActionResult Search(string locationinput)
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.location = locationinput;

            return View();        
        }

        [HttpPost]
        public JsonResult GetLocationData(StreetAddressModel array)
        {
            return null;
        }

    }
}