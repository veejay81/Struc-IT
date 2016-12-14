using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace StructureITWebAPIFrontEnd
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/search/{ArtistName}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
              name: "Pagination",
              routeTemplate: "{controller}/search/{ArtistName}/{Pagenumber}/{PageSize}"
          );
            config.Routes.MapHttpRoute(
               name: "ArtistID",
               routeTemplate: "{controller}/{artist_id}/releases"
           );

            config.Routes.MapHttpRoute(
              name: "AlbumID",
              routeTemplate: "{controller}/{release_id}/albums"
          );
        }
    }
}
