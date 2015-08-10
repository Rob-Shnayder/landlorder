using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using landlorder.Models;
using System.Web.Routing;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity;
using landlorder.ViewModels;
using System.Data.SqlClient;
using System.Data.Sql;
using Geocoding;
using Geocoding.Google;


namespace landlorder.Controllers
{
    public class ReviewsController : Controller
    {
        private landlorderEntities2 db = new landlorderEntities2();

        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = db.Reviews.Include(r => r.Property);
            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var p = db.Properties.Where(x => x.formatted_address == id).Select(x =>
                                                new ReviewViewModel()
                                                {
                                                    propertyID = x.propertyID,
                                                    streetaddress = x.streetaddress,
                                                    route = x.route,
                                                    city = x.city,
                                                    state = x.state,
                                                    zip = x.zip,
                                                    country = x.country,
                                                    latitude = x.latitude,
                                                    longitude = x.longitude,
                                                    reviews = x.Reviews.Select(a=> a.review1)

                                                }).SingleOrDefault();         

            if (p == null)
            {
                ReviewViewModel noreview = new ReviewViewModel();
                noreview.formatted_address = id;
                return View(noreview);
            }
            return View(p);
        }

        // GET: Reviews/Create/1
        [Authorize]
        public ActionResult Create(int? id)
        {
            //ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress");
            //ViewBag.lat = lat;
            //ViewBag.lon = lon;

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Property p = db.Properties.Find(id);
            if (p == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.address = p.formatted_address;

            return View();
        }

        public ActionResult CreateNewProperty(string id)
        {            
            ViewBag.address = id;

            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id,[Bind(Include = "reviewID,rating,review1,apartmentnum,landlordname,repairrating,communicationrating")] Review review)
        {
            if (ModelState.IsValid)
            {
                //CHANGE ID HERE
                var property = db.Properties.FirstOrDefault(p => p.propertyID == id);
                var userID = User.Identity.GetUserId();
                review.propertyID = property.propertyID;
                review.userID = userID;
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }


        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProperty(string id, [Bind(Include = "reviewID,rating,review1,apartmentnum,landlordname,repairrating,communicationrating")] Review review)
        {          

            if (ModelState.IsValid)
            {
                //CHANGE ID HERE
                var property = Geocode(id);

                if (property == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.Properties.Add(property);
                var userID = User.Identity.GetUserId();
                review.propertyID = property.propertyID;
                review.userID = userID;
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "reviewID,propertyID,userID,rating,review1,apartmentnum,landlordname,repairrating,communicationrating")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET
        public ActionResult Search(string locationinput, int? pagenum)
        {
            //When pulling apartments, do a group by on rows that do not have a 
            //APT number entry. For ones that match each other group those together.

            if (pagenum == null)
            {
                pagenum = 1;
            }

            ViewBag.location = locationinput;

            /*List<SearchResultsViewModel> query1 =
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
                    apartmentnum = "",
                    formatted_address = p.formatted_address,
                    latitude = p.latitude,
                    longitude = p.longitude
                    //db.Properties.Include(r => r.Reviews.Select(b=>b.apartmentnum).Where(q=> r.propertyID == p.propertyID ))
                }).ToList();
            */

            return View();
        }


        [HttpPost]
        public JsonResult GetLocationData(StreetAddressModel array)
        {
            List<SearchResultsViewModel> property = null;
            //List<SearchResultsViewModel> relatedproperties = null;

            if (array.type == "street_address")
            {
                property = StreetAddressLocation(array);
            }                      
            
            var relatedproperties = RelatedProperties(array, 1);

            var result = new { property = property, relatedproperties = relatedproperties };
            return Json(result);
        }

        private List<SearchResultsViewModel> StreetAddressLocation(StreetAddressModel array)
        {
            /*
            var property = db.Database.SqlQuery<SearchResultsViewModel>("SearchReviews_StreetAddress @streetaddress, @route, @city,@state,@postal_code",
                    new SqlParameter("@streetaddress", array.street_number),
                    new SqlParameter("@route", array.route),
                    new SqlParameter("@city", array.city),
                    new SqlParameter("@state", array.state),
                    new SqlParameter("@postal_code", array.postal_code)).ToList();
            */

            var property = db.Properties.Where(c => (c.streetaddress == array.street_number)
                && (c.route == array.route)).Select(x => new SearchResultsViewModel
                {
                    formatted_address = x.formatted_address,
                    propertyID = x.propertyID,
                    numofReviews = x.Reviews.Count()
                }).ToList();

            return property;
        }
        public List<SearchResultsViewModel> RelatedProperties(StreetAddressModel array, int pagenum)
        {
            var property = db.Database.SqlQuery<SearchResultsViewModel>("SearchReviews_StreetAddress_Related @lat, @lon, @vicinity,@pagenum",
                   new SqlParameter("@lat", array.latitude),
                   new SqlParameter("@lon", array.longitude),
                   new SqlParameter("@vicinity", array.city),
                   new SqlParameter("@pagenum", pagenum)).ToList();

            return property;
        }
        
        
        //Geocode functions
        public void GetProp()
        {
            decimal blank = 0.0m;

            var p = db.Properties.Where(c => c.latitude == blank).Select(x => new ReviewViewModel
                                               {                                                    
                                                   formatted_address = x.formatted_address,
                                                   propertyID =x.propertyID
                                               }).ToList();

            //Geocode(p);
            
        }
        public void StoreLatLng(decimal lat, decimal lng, int id)
        {

            try
            {
                Property c = (from x in db.Properties
                              where x.propertyID == id
                              select x).First();
                c.latitude = lat;
                c.longitude = lng;
                

                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
        
        private Property Geocode(string address)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            GoogleGeocoder geocoder = new GoogleGeocoder();
            IEnumerable<GoogleAddress> addresses = geocoder.Geocode(address);

            if (addresses == null)
            {
                return null; 
            }

            var p = addresses.Where(a => !a.IsPartialMatch).Select
                    (a => new Property()
                    {
                        streetaddress = a[GoogleAddressType.StreetNumber].ShortName,
                        route = a[GoogleAddressType.Route].LongName,
                        state = a[GoogleAddressType.AdministrativeAreaLevel1].LongName,
                        zip = a[GoogleAddressType.PostalCode].LongName,
                        city = a[GoogleAddressType.Locality].ShortName,
                        country = a[GoogleAddressType.Country].LongName,
                        latitude = (decimal)addresses.First().Coordinates.Latitude,
                        longitude = (decimal)addresses.First().Coordinates.Longitude,
                        formatted_address = addresses.First().FormattedAddress
                    }
                    ).First();

            return p;
            
        }





    }
}
