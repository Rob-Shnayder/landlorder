﻿using System;
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
using PagedList;
using PagedList.Mvc;


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
        public ActionResult DetailsNew(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.address = id;

            ReviewViewModel noreview = new ReviewViewModel();
            noreview.formatted_address = id;
            return View(noreview);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*
            var p = db.Properties.Where(x => x.propertyID == id).Select(x =>
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
                                                     reviews = x.Reviews.Select(a => a.review1),
                                                     formatted_address = x.formatted_address

                                                 }).ToList();         
             */

            var p = db.Reviews.Where(x => x.propertyID == id).ToList();
 

            if (p == null)
            {
                return HttpNotFound();
            }
            ViewBag.address = p.First().Property.formatted_address;

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
                var property = Geocode_Create(id);

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
            if (pagenum == null){ pagenum = 1; }
            var pageIndex = (pagenum ?? 1) - 1; 
            var pageSize = 10;
            int skip = pageSize * pageIndex;
            
            //Geocode input address
            var geocodedAddress = Geocode(locationinput);
            if (geocodedAddress == null)
            {
                return null;
            }

            SearchResultsViewModel exactproperty = null;
            if (pagenum == 1) { exactproperty = SearchExactAddress(geocodedAddress); }

            var relatedproperties = RelatedProperties(geocodedAddress, 1);

            if (exactproperty != null) 
            { 
                relatedproperties.Insert(0, exactproperty); 
            }
            
            //Create one page of results
            var results = new StaticPagedList<SearchResultsViewModel>(relatedproperties.Skip(skip).Take(pageSize), 
                pageIndex + 1, pageSize, relatedproperties.Count());
                   
            results = GetLocationDataForRelated(results);


            ViewBag.address = geocodedAddress.formatted_address;
            ViewBag.input = locationinput;
            

            return View(results);
        }

        private StaticPagedList<SearchResultsViewModel> GetLocationDataForRelated(StaticPagedList<SearchResultsViewModel> relatedproperties)
        {            
            Geography locationData;

            for(int i = 0; i < relatedproperties.Count(); i++)
            {
                relatedproperties[i].type = "related";

                locationData = GetLatLng(relatedproperties[i].formatted_address);
                if (locationData != null)
                {
                    relatedproperties[i].latitude = locationData.latitude;
                    relatedproperties[i].longitude = locationData.longitude;
                }
            }

            return relatedproperties;
        }

        private SearchResultsViewModel SearchExactAddress(SearchCompare geocodedAddress)
        {
            Geography locationData;
            var exactproperty = StreetAddressLocation(geocodedAddress);
            if (exactproperty != null)
            {
                exactproperty.type = "exact";

                if (exactproperty.latitude == 0m)
                {
                    locationData = GetLatLng(exactproperty.formatted_address);
                    if (locationData != null)
                    {
                        exactproperty.latitude = locationData.latitude;
                        exactproperty.longitude = locationData.longitude;
                    }
                }
                return exactproperty;
            }
            else
            {
                SearchResultsViewModel V1 = new SearchResultsViewModel();
                V1.formatted_address = geocodedAddress.formatted_address;
                V1.latitude = geocodedAddress.latitude;
                V1.longitude = geocodedAddress.longitude;
                V1.type = "exact-new";

                return V1;
            }


        }

        public JsonResult GetLocationData(StreetAddressModel array)
        {
            /*
            List<SearchResultsViewModel> property = null;
            //List<SearchResultsViewModel> relatedproperties = null;

            if (array.type == "street_address")
            {
                //property = StreetAddressLocation(array);
            }                      
            
            //var relatedproperties = RelatedProperties(array, 1);

            var result = new { property = property, relatedproperties = relatedproperties };
            return Json(result);
             */
            return null;
        }



        private SearchResultsViewModel StreetAddressLocation(SearchCompare array)
        {
            /*
            var property = db.Database.SqlQuery<SearchResultsViewModel>("SearchReviews_StreetAddress @streetaddress, @route, @city,@state,@postal_code",
                    new SqlParameter("@streetaddress", array.street_number),
                    new SqlParameter("@route", array.route),
                    new SqlParameter("@city", array.city),
                    new SqlParameter("@state", array.state),
                    new SqlParameter("@postal_code", array.postal_code)).ToList();
            */

            var property = db.Properties.Where(c => (c.streetaddress == array.streetaddress)
                && ((c.route == array.route) || (c.route == array.route_long))).Select(x => new SearchResultsViewModel
                {
                    streetaddress = x.streetaddress,
                    route = x.route,
                    city = x.city,
                    zip= x.zip,
                    state = x.state,
                    country = x.country,
                    formatted_address = x.formatted_address,
                    propertyID = x.propertyID,
                    numofReviews = x.Reviews.Count(),
                    type = "exact"
                }).FirstOrDefault();

            return property;
        }
        public List<SearchResultsViewModel> RelatedProperties(SearchCompare array, int pagenum)
        {
            var property = db.Database.SqlQuery<SearchResultsViewModel>("SearchReviews_StreetAddress_Related @lat, @lon, @vicinity,@pagenum",
                   new SqlParameter("@lat", array.latitude),
                   new SqlParameter("@lon", array.longitude),
                   new SqlParameter("@vicinity", array.city),
                   new SqlParameter("@pagenum", pagenum)).ToList();

            return property;
        }
        
        
        //Geocode functions
        private SearchCompare Geocode(string address)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            GoogleGeocoder geocoder = new GoogleGeocoder();
            IEnumerable<GoogleAddress> addresses = geocoder.Geocode(address);

            if (addresses == null)
            {
                return null; 
            }

            var p = addresses.Where(a => !a.IsPartialMatch).Select
                    (a => new SearchCompare()
                    {
                        streetaddress = a[GoogleAddressType.StreetNumber].ShortName != null ? a[GoogleAddressType.StreetNumber].ShortName : string.Empty,
                        route = a[GoogleAddressType.Route].ShortName,
                        state = a[GoogleAddressType.AdministrativeAreaLevel1].LongName,
                        zip = a[GoogleAddressType.PostalCode].LongName,
                        city = a[GoogleAddressType.Locality].ShortName,
                        country = a[GoogleAddressType.Country].ShortName,
                        latitude = (decimal)addresses.First().Coordinates.Latitude,
                        longitude = (decimal)addresses.First().Coordinates.Longitude,
                        route_long = a[GoogleAddressType.Route].LongName                                        
                    }
                    ).FirstOrDefault();

            p.formatted_address = p.streetaddress + " " + p.route + ", " + p.city + ", " + p.state + ", " + p.zip + ", " + p.country;

            return p;
            
        }

        private Property Geocode_Create(string address)
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
                        route = a[GoogleAddressType.Route].ShortName,
                        state = a[GoogleAddressType.AdministrativeAreaLevel1].LongName,
                        zip = a[GoogleAddressType.PostalCode].LongName,
                        city = a[GoogleAddressType.Locality].ShortName,
                        country = a[GoogleAddressType.Country].ShortName,
                        latitude = (decimal)addresses.First().Coordinates.Latitude,
                        longitude = (decimal)addresses.First().Coordinates.Longitude
                    }
                    ).First();

            p.formatted_address = p.streetaddress + " " + p.route + ", " + p.city + ", " + p.state + ", " + p.zip + ", " + p.country;

            return p;

        }

        private Geography GetLatLng(string address)
        {
            if (address == null)
            {
                return null;
            }

            System.Net.ServicePointManager.Expect100Continue = false;

            GoogleGeocoder geocoder = new GoogleGeocoder();
            IEnumerable<Address> addresses = geocoder.Geocode(address);

            if (addresses == null)
            {
                return null;
            }

            Geography G1 = new Geography();
            try
            {
                G1.latitude = (decimal)addresses.First().Coordinates.Latitude;
                G1.longitude = (decimal)addresses.First().Coordinates.Longitude;
            }
            catch
            {
                G1.latitude = 0.0m;
                G1.latitude = 0.0m;
            }

            return G1;

        }





    }
}
