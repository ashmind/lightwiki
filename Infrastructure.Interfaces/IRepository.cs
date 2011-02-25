using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Interfaces {
    public interface IRepository<T> {
        T Load(string key);
        void Save(T entity);

        IEnumerable<T> Query();
    }
}
