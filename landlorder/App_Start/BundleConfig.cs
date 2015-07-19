using System.Web;
using System.Web.Optimization;

namespace landlorder
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {            

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                "~/scripts/jquery-1.11.1.min.js",
                "~/scripts/jquery-migrate-1.2.1.min.js",
                "~/scripts/jquery.easing.1.3.js",
                "~/scripts/bootstrap/js/bootstrap.min.js",
                "~/scripts/jquery-scrollTo/jquery.scrollTo.min.js",
                "~/scripts/login.js"));          

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/scripts/bootstrap/css/bootstrap.min.css",                      
                      "~/content/font-awesome/css/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/login.css",
                       "~/Content/Review.css"));

            bundles.Add(new ScriptBundle("~/bundles/googleaddressJS").Include(
                       "~/Scripts/googleaddress.js"));

            bundles.Add(new ScriptBundle("~/bundles/googlemapsJS").Include(
                       "~/Scripts/googlemaps.js"));
        }
    }
}
