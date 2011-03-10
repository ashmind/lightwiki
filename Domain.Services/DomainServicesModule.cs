using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using AshMind.LightWiki.Domain.Services.Concurrency;
using AshMind.LightWiki.Domain.Services.Syntax;

namespace AshMind.LightWiki.Domain.Services {
    public class DomainServicesModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<DiffMatchPatch.diff_match_patch>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<LightSyntax>()
                   .As<IWikiSyntax>()
                   .SingleInstance();

            builder.RegisterType<WikiPageUpdater>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<LinkBasedHierarchyProvider>()
                   .As<IWikiPageHierarchyProvider>()
                   .SingleInstance();

            WikiPage.Create = (slug, text) => new VersionedWikiPage(slug, text);

            base.Load(builder);
        }
    }
}
