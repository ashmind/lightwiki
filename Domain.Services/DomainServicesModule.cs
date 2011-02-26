using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

namespace AshMind.LightWiki.Domain.Services {
    public class DomainServicesModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<DiffMatchPatch.diff_match_patch>()
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<WikiPageUpdater>()
                   .AsSelf()
                   .SingleInstance();

            base.Load(builder);
        }
    }
}
