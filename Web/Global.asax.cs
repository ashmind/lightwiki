using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;

using AshMind.Extensions;

using AshMind.Web.Mvc;
using AshMind.Web.Mvc.Routing;

using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.Web.Mvc.KeyModel;

namespace AshMind.LightWiki.Web {
    public class MvcApplication : ConfiguredMvcApplicationBase {
        protected override void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapLowerCaseRoute(
                "Default",
                "",
                new { controller = "Wiki", action = "View", Slug = "root" }
            );

            routes.MapLowerCaseRoute(
                "Wiki",
                "wiki/{slug}",
                new { controller = "Wiki", action = "View" }
            );
        }

        protected override void RegisterContainer() {
            base.RegisterContainer();
            RequestScope.Resolve<IApplicationSetup[]>().ForEach(s => s.Setup());
        }

        protected override bool ShouldDiscoverModulesIn(string path) {
            return path.Contains("AshMind.LightWiki.");
        }
    }
}