using System;

using Autofac;

namespace AshMind.Web.Mvc.Configuration {
    public interface IModuleDiscovery {
        void Discover(ContainerBuilder builder);
    }
}
