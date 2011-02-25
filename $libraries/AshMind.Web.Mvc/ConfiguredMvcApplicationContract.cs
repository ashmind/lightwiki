using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Routing;

namespace AshMind.Web.Mvc {
    [ContractClassFor(typeof(ConfiguredMvcApplicationBase))]
    internal abstract class ConfiguredMvcApplicationContract : ConfiguredMvcApplicationBase {
        protected override void RegisterRoutes(RouteCollection routes) {
            Contract.Requires<ArgumentNullException>(routes != null);
            throw new NotSupportedException();
        }
    }
}
