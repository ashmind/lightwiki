using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using Autofac;

using Module = Autofac.Module;

using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Web.Handlers;
using AshMind.LightWiki.Web.Syntax;
using AshMind.LightWiki.Web.Urls;

namespace AshMind.LightWiki.Web {
    public class WebModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .InNamespaceOf<WikiHandler>()
                   .AsImplementedInterfaces()
                   .AsSelf();

            builder.RegisterType<HtmlWikiOutputFormat>()
                   .As<IWikiOutputFormat>()
                   .AsSelf()
                   .SingleInstance();

            builder.Register(c => new SimpleWikiUrlProvider(HttpRuntime.AppDomainAppVirtualPath))
                   .As<IWikiUrlProvider>()
                   .SingleInstance();
        }
    }
}