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
using System.Device.Location;


namespace landlorder.Controllers
{
    public class ReviewsController : Controller
    {
        private landlorderEntities2 db = new landlorderEntities2();

        //*************************
        //SEARCH Functions
        //*************************

        //GET
        public ActionResult Search(string locationinput, int? pagenum)
        {
            if (locationinput == null) { return RedirectToAction("Index", "Home");}
            if (pagenum == null) { pagenum = 1; }

            //Geocode input address
            SearchCompare geocodedAddress = Geocode(locationinput);
            if (geocodedAddress == null) { return null; }
            

            SearchResultsViewModel exactproperty = new SearchResultsViewModel();         
            List<SearchResultsViewModel> relatedproperties = null;

            if (pagenum == 1 && geocodedAddress.type == GoogleAddressType.StreetAddress) {
                exactproperty = ConfigureExactAddressForView(geocodedAddress);
            }

            relatedproperties = SearchAllRelatedProperties(geocodedAddress, exactproperty.propertyID);

            if (exactproperty.type != null) {
                relatedproperties.Insert(0, exactproperty);
            }
            
            var results = OnePageOfResults(relatedproperties, pagenum);
            ViewBag.address = geocodedAddress.formatted_address;
            return View(results);
        }
    
        private StaticPagedList<SearchResultsViewModel> OnePageOfResults(List<SearchResultsViewModel> r,int? pagenum)
        {
            var pageIndex = (pagenum ?? 1) - 1;
            var pageSize = 8;
            int skip = pageSize * pageIndex;

            var results = new StaticPagedList<SearchResultsViewModel>(r.Skip(skip).Take(pageSize),
                pageIndex + 1, pageSize, r.Count());

            results = GetLocationDataForRelated(results);

            return results;
        }

        private StaticPagedList<Review> OnePageOfResults(List<Review> r, int? pagenum)
        {
            var pageIndex = (pagenum ?? 1) - 1;
            var pageSize = 15;
            int skip = pageSize * pageIndex;

            var results = new StaticPagedList<Review>(r.Skip(skip).Take(pageSize),
                pageIndex + 1, pageSize, r.Count());

            return results;
        }


        //*************************
        //DETAIL Functions
        //*************************

        public ActionResult DetailsNew(string id, decimal lat, decimal lng)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DetailsViewModel noreview = new DetailsViewModel
            {
                propertyID = 0,
                formatted_address = id,
                overallRating = 0.0,
                numOfReviews = 0,
                repairRating = 0.0,
                communicationRating = 0.0,
                latitude = lat,
                longitude = lng,
                Reviews = null
            };

            ViewBag.address = id;

            return View(noreview);
        }
        public ActionResult Details(int? id, int? pagenum)
        {
            if (id == null) { return RedirectToAction("Index", "Home"); }
            if (pagenum == null) { pagenum = 1; }

            
            var p = db.Properties.Where(x => x.propertyID == id).Select(a => new DetailsViewModel
                {

                    propertyID = a.propertyID,
                    formatted_address = a.formatted_address,
                    latitude = a.latitude,
                    longitude = a.longitude,
                    numOfReviews = a.Reviews.Count(),
                    overallRating = (double?)(a.Reviews.Select(b => b.rating).Average()) ?? 0.0,
                    repairRating = (double?)(a.Reviews.Select(b => b.repairrating).Average()) ?? 0.0,
                    communicationRating = (double?)(a.Reviews.Select(b => b.communicationrating).Average()) ?? 0.0 ,
                    Reviews = a.Reviews.OrderByDescending(x=>x.date),
                    users = db.AspNetUsers.Where((c=>c.Id == 
                        (c.Reviews.Where(d=> d.propertyID == a.propertyID).Select(e=>e.userID).FirstOrDefault()))).FirstOrDefault()
                }).FirstOrDefault(); 

            if (p == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (p.latitude == 0m)
            {
                var locationData = GetLatLng(p.formatted_address);
                p.latitude = locationData.latitude;
                p.longitude = locationData.longitude;
            }

            p.PagedReviews = OnePageOfResults(p.Reviews.ToList(), pagenum);

            ViewBag.address = p.formatted_address;
            return View(p);
        }


        //*************************
        //CREATE Functions
        //*************************

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
        [Authorize]
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
        public ActionResult Create(int id,[Bind(Include = "reviewID,rating,review1,apartmentnum,landlordname,repairrating,communicationrating, anonymous")] Review review)
        {
            if (ModelState.IsValid)
            {
                //CHANGE ID HERE
                var property = db.Properties.FirstOrDefault(p => p.propertyID == id);
                var userID = User.Identity.GetUserId();
                review.propertyID = property.propertyID;
                review.userID = userID;
                review.date = DateTime.Now;
                if (ModelState.ContainsKey("anonymous"))
                {
                    if (ModelState["anonymous"].Value.AttemptedValue == "true")
                    {
                        review.anonymous = true;
                    }
                    else
                    {
                        review.anonymous = false;
                    }
                }
                db.Reviews.Add(review);
                db.SaveChanges();



                int tempid = property.propertyID;
                return RedirectToAction("Details", "Reviews", new { id = tempid });
            }

            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProperty(string id, [Bind(Include = "reviewID,rating,review1,apartmentnum,landlordname,repairrating,communicationrating, anonymous")] Review review)
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
                review.date = DateTime.Now;
                db.Reviews.Add(review);
                db.SaveChanges();

                int tempid = property.propertyID;
                return RedirectToAction("Details", "Reviews", new { id = tempid });
            }

            ViewBag.propertyID = new SelectList(db.Properties, "propertyID", "streetaddress", review.propertyID);
            return View(review);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //*************************
        //HELPER Functions
        //*************************

        //Gets latitude and longitude data for properties that may not have that data already
        private StaticPagedList<SearchResultsViewModel> GetLocationDataForRelated(StaticPagedList<SearchResultsViewModel> relatedproperties)
        {            
            Geography locationData;

            for(int i = 0; i < relatedproperties.Count(); i++)
            {
                if (string.IsNullOrEmpty(relatedproperties[i].type))
                {
                    relatedproperties[i].type = "related";
                }

                locationData = GetLatLng(relatedproperties[i].formatted_address);
                if (locationData != null)
                {
                    relatedproperties[i].latitude = locationData.latitude;
                    relatedproperties[i].longitude = locationData.longitude;
                }
            }

            return relatedproperties;
        }


        //Prepares the exact address property object for view
        //If the address exists in DB, configure its lat/lng
        //If it doesnt, configure it for the view and give its needed data for the view.
        private SearchResultsViewModel ConfigureExactAddressForView(SearchCompare geocodedAddress)
        {
            Geography locationData;
            var exactproperty = SearchForExactAddressLocation(geocodedAddress);
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
                V1.propertyID = 0;
                V1.formatted_address = geocodedAddress.formatted_address;
                V1.latitude = geocodedAddress.latitude;
                V1.longitude = geocodedAddress.longitude;
                V1.type = "exact-new";

                return V1;
            }


        }
        
        //Searchs for properties in DB
        private SearchResultsViewModel SearchForExactAddressLocation(SearchCompare array)
        {
            var property = db.Properties.Where(c => (c.streetaddress == array.streetaddress)
                && ((c.route == array.route) || (c.route == array.route_long))).Select(x => new SearchResultsViewModel
                {                   
                    formatted_address = x.formatted_address,
                    propertyID = x.propertyID,
                    numofReviews = x.Reviews.Count(),
                    averagerating = (double?)(x.Reviews.Select(b => b.rating).Average()) ?? 0.0
                }).FirstOrDefault();

            return property;
        }
        private List<SearchResultsViewModel> SearchAllRelatedProperties(SearchCompare array, int id)
        {
            
            var coord = new GeoCoordinate { Latitude = (double?)array.latitude ?? 0, Longitude = (double?)array.longitude ?? 0 };
            var property = db.Properties.Select(x => new SearchResultsViewModel
                {
                    propertyID = x.propertyID,
                    formatted_address = x.formatted_address,
                    numofReviews = x.Reviews.Count(),
                    averagerating = (double?)(x.Reviews.Select(b => b.rating).Average()) ?? 0.0,
                    latitude = x.latitude,
                    longitude = x.longitude,
                    geocoord = new GeoCoordinate { Latitude = (double?)x.latitude ?? 0, Longitude = (double?)x.longitude ?? 0 },
                    type = "exact"
                })    
                .Where(x => x.propertyID != id)
                .AsEnumerable()
                .OrderBy(x => x.geocoord.GetDistanceTo(coord))
                .Take(500).ToList();

             
            /*
            var property = db.Database.SqlQuery<Property>("SearchReviews_StreetAddress_Related @lat, @lon, @propertyid",
            new SqlParameter("@lat", array.latitude),
            new SqlParameter("@lon", array.longitude),
            new SqlParameter("@propertyid", array.propertyID)).
           .ToList();
             * */
            
            return property;
        }
        
        
        //Geocode functions
        private SearchCompare Geocode(string address)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            GoogleGeocoder geocoder = new GoogleGeocoder();
            IEnumerable<GoogleAddress> addresses = geocoder.Geocode(address);

            if (addresses != null)
            {
                var results = MapProperties(addresses);
                return results;
            }
            else
            {
                return null;
            }            
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

            Geography G1 = new Geography();
            if (addresses == null)
            {
                G1.latitude = 0.0m;
                G1.latitude = 0.0m;
                return G1;
            }

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


        private SearchCompare MapProperties(IEnumerable<GoogleAddress> g)
        {
            var results = new SearchCompare();

            foreach (var item in g.First().Components)
            {
                if (item.Types.First() == GoogleAddressType.StreetNumber) { results.streetaddress = item.ShortName; }
                if (item.Types.First() == GoogleAddressType.Route) { results.route = item.ShortName; results.route_long = item.LongName; }
                if (item.Types.First() == GoogleAddressType.Locality) { results.city = item.ShortName; results.city_long = item.LongName; }
                if (item.Types.First() == GoogleAddressType.AdministrativeAreaLevel1) { results.state = item.ShortName; }
                if (item.Types.First() == GoogleAddressType.Country) { results.country = item.ShortName; }
                if (item.Types.First() == GoogleAddressType.PostalCode) { results.zip = item.ShortName; }
            }
            results.formatted_address = g.First().FormattedAddress;
            results.latitude = (decimal)g.First().Coordinates.Latitude;
            results.longitude = (decimal)g.First().Coordinates.Longitude;
            results.type = g.First().Type;


            return results;
        }


    }
}
