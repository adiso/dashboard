using System.Web.Mvc;
using System.Web.Routing;

namespace DapperDashboard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Add route for invoice details by reference
            routes.MapRoute(
                name: "InvoiceDetails",
                url: "Invoice/Details/{ref}",
                defaults: new { controller = "Invoice", action = "Details", @ref = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           
        }
    }
}