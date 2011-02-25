using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction;
using AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction.Implementation;

namespace AshMind.LightWiki.Infrastructure.Repositories {
    public class RepositoriesModule : Module {
        protected override void Load(ContainerBuilder builder) {
            var system = new FileSystem();
            builder.Register(c => new WikiPageRepository(
                        system.GetLocation(c.Resolve<RepositoryOptions>().StorageRootPath, ActionIfMissing.CreateNew)
                   ))
                   .As<IRepository<WikiPage>>()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}
