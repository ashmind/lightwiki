using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AshMind.LightWiki.Infrastructure.Interfaces {
    public interface IRepository<T> {
        T Load(string key);
        void Save(T entity);
    }
}
