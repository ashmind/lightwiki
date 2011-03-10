using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using AshMind.LightWiki.Infrastructure.Interfaces.Contracts;

namespace AshMind.LightWiki.Infrastructure.Interfaces {
    [ContractClass(typeof(RepositoryContract<>))]
    public interface IRepository<T> {
        T Load(string key);
        void Save(T entity);

        IEnumerable<T> Query();
    }
}
