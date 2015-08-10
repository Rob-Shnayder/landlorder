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
                "~/scripts/bootstrap/js/bootstrap.min.js" ));   

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/scripts/bootstrap/css/bootstrap.min.css",                      
                      "~/content/font-awesome/css/font-awesome.css",
                      "~/Content/site.css",
                      "~/Content/login.css",
                       "~/Content/Review.css"));

            bundles.Add(new ScriptBundle("~/bundles/googleaddressJS").Include(
                       "~/Scripts/googleaddress.js"));


        }
    }
}
