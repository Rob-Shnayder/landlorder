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

        public ActionResult Search(string locationinput)
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.location = locationinput;

            return View();        
        }

        [HttpPost]
        public JsonResult GetLocationData(StreetAddressModel array)
        {
            using (var context = new landlorderEntities2())
            {
                var affectedRows = context.Database.ExecuteSqlCommand("usp_CreateAuthor @AuthorName, @Email",
                    new SqlParameter("@AuthorName", "author"),
                    new SqlParameter("@Email", "email"));
            }
            return null;
        }

    }
}