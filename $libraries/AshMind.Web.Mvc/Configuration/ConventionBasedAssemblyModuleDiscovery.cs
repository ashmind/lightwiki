using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Core;

using AshMind.Extensions;

namespace AshMind.Web.Mvc.Configuration {
    public class ConventionBasedAssemblyModuleDiscovery : IModuleDiscovery {
        private readonly DirectoryInfo root;
        private readonly Func<FileInfo, bool> assemblyFileFilter;

        public ConventionBasedAssemblyModuleDiscovery(DirectoryInfo root, Func<FileInfo, bool> assemblyFileFilter) {
            this.root = root;
            this.assemblyFileFilter = assemblyFileFilter;
        }

        public virtual void Discover(ContainerBuilder builder) {
            foreach (var file in root.GetFiles("*.dll")) {
                if (!this.assemblyFileFilter(file))
                    continue;

                var assembly = Assembly.LoadFrom(file.FullName);
                Contract.Assume(assembly != null);
                var modules = from type in assembly.GetTypes()
                              where typeof(IModule).IsAssignableFrom(type)
                              select (IModule)Activator.CreateInstance(type);

                modules.ForEach(builder.RegisterModule);
            }
        }
    }
}
