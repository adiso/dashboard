using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DapperDashboard;
using System.Web.Helpers;
using System.Security.Claims;

namespace DapperDashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // ...

            // --- ADD THIS LINE ---
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "CustomerID";


        }
    }
}
