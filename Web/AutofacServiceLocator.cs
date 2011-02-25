using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using Microsoft.Practices.ServiceLocation;

namespace AshMind.LightWiki.Web {
    public sealed class AutofacServiceLocator : ServiceLocatorImplBase {
        private readonly Func<ILifetimeScope> lifetimeScopeProvider;

        public AutofacServiceLocator(Func<ILifetimeScope> lifetimeScopeProvider) {
            if (lifetimeScopeProvider == null)
                throw new ArgumentNullException("lifetimeScopeProvider");

            this.lifetimeScopeProvider = lifetimeScopeProvider;
        }

        protected override object DoGetInstance(Type serviceType, string key) {
            if (key != null)
                return this.lifetimeScopeProvider().ResolveNamed(key, serviceType);

            return this.lifetimeScopeProvider().Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType) {
            return ((IEnumerable)this.lifetimeScopeProvider().Resolve(
                typeof(IEnumerable<>).MakeGenericType(serviceType)
            )).Cast<object>();
        }
    }
}