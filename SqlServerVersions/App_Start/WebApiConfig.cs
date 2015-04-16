using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SqlServerVersions
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // route for a version query
            //
            config.Routes.MapHttpRoute(
                "BaseVersionApi",
                "api/version/{major}/{minor}/{build}/{revision}",
                new
                {
                    controller = "Version",
                    action = "SearchVersion",
                    major = RouteParameter.Optional,
                    minor = RouteParameter.Optional,
                    build = RouteParameter.Optional,
                    revision = RouteParameter.Optional
                }
            );

            // route for a version query
            //
            config.Routes.MapHttpRoute(
                "TopVersionApi",
                "api/recent/{topcount}",
                new 
                { 
                    controller = "Version",
                    action = "TopRecentRelease",
                    topcount = 5
                }
            );

            // route for a version query
            //
            config.Routes.MapHttpRoute(
                "LatestVersionApi",
                "api/latest/{major}/{minor}",
                new
                {
                    controller = "Version",
                    action = "MostRecentByMajorMinor"
                }
            );

            // route for a version query
            //
            config.Routes.MapHttpRoute(
                "OldestSupportedVersionApi",
                "api/oldestsupported/{major}/{minor}",
                new
                {
                    controller = "Version",
                    action = "LowestSupportedByMajorMinor"
                }
            );

            // route for all major and minor versions
            //
            config.Routes.MapHttpRoute(
                "AllMajorMinorReleases",
                "api/allmajorreleases",
                new 
                { 
                    controller = "Version",
                    action = "MajorMinorReleases"
                }
            );

            // route for all recent/oldest supported versions
            //
            config.Routes.MapHttpRoute(
                "SupportBoundaries",
                "api/supportboundaries",
                new
                {
                    controller = "Version",
                    action = "RecentAndOldestSupportedVersions"
                }
            );
            
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
