using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AshMind.LightWiki.Infrastructure.Interfaces.Contracts {
    [ContractClassFor(typeof(IRepository<>))]
    internal abstract class RepositoryContract<T> : IRepository<T> {
        public T Load(string key) {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(key));
            throw new NotSupportedException();
        }

        public void Save(T entity) {
            Contract.Requires<ArgumentNullException>(entity != null);
            throw new NotSupportedException();
        }

        public IEnumerable<T> Query() {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
            throw new NotSupportedException();
        }
    }
}
