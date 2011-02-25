using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Autofac;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Infrastructure.Repositories {
    public class RepositoriesModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<WikiPageRepository>()
                   .As<IRepository<WikiPage>>()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}
