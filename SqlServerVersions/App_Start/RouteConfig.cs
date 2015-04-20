using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SqlServerVersions
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // version search route
            //
            routes.MapRoute(
                "VersionSearch",
                "{major}/{minor}/{build}/{revision}",
                new 
                { 
                    controller = "Home",
                    action = "VersionSearch"
                }
            );

            // most recent route
            //
            routes.MapRoute(
                "MostRecent",
                "recent/{major}/{minor}",
                new
                {
                    controller = "Home",
                    action = "RecentRelease",
                    major = 0,
                    minor = 0
                }
            );

            // supportability route
            //
            routes.MapRoute(
                "Supportability",
                "supported/{major}/{minor}",
                new 
                { 
                    controller = "Home",
                    action = "Supportability",
                    major = 0,
                    minor = 0
                }
            );

            // backfill route
            //
            routes.MapRoute(
                "BackFill",
                "backfill",
                new
                {
                    controller = "Home",
                    action = "BackFill"
                }
            );

            // default route
            //
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
