using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Practices.ServiceLocation;

using AspComet;
using AspComet.Eventing;

using Autofac;

using AshMind.Extensions;

using AshMind.Web.Mvc;
using AshMind.Web.Mvc.Routing;

using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Web.Handlers;

namespace AshMind.LightWiki.Web {
    public class MvcApplication : ConfiguredMvcApplicationBase {
        protected override void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapLowerCaseRoute(
                "Default",
                "",
                new { controller = "Wiki", action = "Main", Slug = "root" }
            );

            routes.MapRoute(
                "Wiki",
                "wiki/{slug}",
                new { controller = "Wiki", action = "Main" }
            );
        }

        protected override void BuildContainer(ContainerBuilder builder) {
            base.BuildContainer(builder);
            foreach (var metadata in ServiceMetadata.GetMinimumSet()) {
                var register = builder.RegisterType(metadata.ActualType).As(metadata.ServiceType);
                if (!metadata.IsPerRequest)
                    register.SingleInstance();
            }

            builder.RegisterInstance(new Infrastructure.Repositories.RepositoryOptions {
                StorageRootPath = Server.MapPath("~/#Wiki")
            });
        }

        protected override void SetupContainer() {
            base.SetupContainer();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(() => RequestScope));
            Container.Resolve<IApplicationSetup[]>().ForEach(s => s.Setup());

            EventHub.Subscribe<PublishingEvent>(
                "/wiki/**", Container.Resolve<WikiHandler>().ProcessEvent
            );
        }
        
        protected override bool ShouldDiscoverModulesIn(FileInfo file) {
            return file.Name.StartsWith("AshMind.LightWiki.");
        }
    }
}