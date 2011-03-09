using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.LightWiki.Domain.Services.Concurrency;

using Autofac;

namespace AshMind.LightWiki.Domain.Services {
    public class DomainServicesModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<DiffMatchPatch.diff_match_patch>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<Syntax.WikiSyntaxTransformer>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<WikiPageUpdater>()
                   .AsSelf()
                   .SingleInstance();

            WikiPage.Create = (slug, text) => new VersionedWikiPage(slug, text);

            base.Load(builder);
        }
    }
}
