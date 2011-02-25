using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

using AshMind.Extensions;
using System.Diagnostics.Contracts;
using AshMind.Web.Mvc.Configuration;

namespace AshMind.Web.Mvc {
    [ContractClass(typeof(ConfiguredMvcApplicationContract))]
    public abstract class ConfiguredMvcApplicationBase : HttpApplication {
        private static IContainer container;
        private static AutofacDependencyResolver dependencyResolver;

        private static Exception startFailure;

        public static ILifetimeScope RequestScope {
            get { return dependencyResolver.RequestLifetimeScope; }
        }

        protected static IContainer Container {
            get { return container; }
        }

        protected void Application_Start() {
            Start();
        }

        protected void Start() {
            try {
                this.SetupBeforeAll();

                AreaRegistration.RegisterAllAreas();

                Contract.Assume(RouteTable.Routes != null);
                this.RegisterRoutes(RouteTable.Routes);
                this.SetupContainer();

                this.SetupAfterAll();

                startFailure = null;
            }
            catch (Exception ex) {
                startFailure = ex;         
            }
        }

        private void Restart() {
            Contract.Assume(RouteTable.Routes != null);
            RouteTable.Routes.Clear();
            this.Start();
        }

        protected virtual void SetupBeforeAll() {
        }

        protected abstract void RegisterRoutes(RouteCollection routes);
        protected virtual void SetupContainer() {
            var builder = new ContainerBuilder();
            this.BuildContainer(builder);

            container = builder.Build();
            dependencyResolver = new AutofacDependencyResolver(container);

            Contract.Assume(ControllerBuilder.Current != null);
            DependencyResolver.SetResolver(dependencyResolver);
        }

        protected virtual void BuildContainer(ContainerBuilder builder) {
            builder.RegisterControllers(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray()).PropertiesAutowired();
            DiscoverAllModules(builder);
        }

        protected virtual void SetupAfterAll() {
        }

        protected virtual void DiscoverAllModules(ContainerBuilder builder) {
            this.CreateModuleDiscovery().Discover(builder);
        }

        protected virtual IModuleDiscovery CreateModuleDiscovery() {
            Contract.Assume(Server != null);
            return new ConventionBasedAssemblyModuleDiscovery(
                new DirectoryInfo(Server.MapPath("~/bin")), ShouldDiscoverModulesIn
            );
        }

        protected virtual bool ShouldDiscoverModulesIn(FileInfo file) {
            Contract.Requires<ArgumentNullException>(file != null);
            return file.Name.StartsWith(this.GetType().Namespace.SubstringBefore(".") + ".");
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            if (startFailure != null)
                this.Restart();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (startFailure != null) {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Critical failure while starting up the application.");
                Response.Output.WriteLine();
                Response.Output.Write(startFailure.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
}
