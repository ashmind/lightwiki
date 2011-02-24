using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Infrastructure.Repositories {
    public class WikiPageRepository : IRepository<WikiPage> {
        public WikiPage Load(string key) {
            throw new NotImplementedException();
        }

        public void Save(WikiPage entity) {
            throw new NotImplementedException();
        }
    }
}