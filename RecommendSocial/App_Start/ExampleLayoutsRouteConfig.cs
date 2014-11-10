using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BootstrapMvcSample.Controllers;
using NavigationRoutes;

namespace BootstrapMvcSample
{
    public class ExampleLayoutsRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute<GoogleController>("Google SignIn", c => c.SignIn());
            routes.MapNavigationRoute<GoogleController>("Rotten Tomatoes", c => c.RottenToematoes());
            routes.MapNavigationRoute<GoogleController>("Movies", c => c.Movies());
            routes.MapNavigationRoute<GoogleController>("Playlists", c => c.Playlists());
            routes.MapNavigationRoute<GoogleController>("Store playlists", c => c.StorePlaylists());
        }
    }
}
