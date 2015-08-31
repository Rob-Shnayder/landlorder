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
        private landlorderEntities2 db = new landlorderEntities2();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Legal()
        {
            return View();
        }


    }
}