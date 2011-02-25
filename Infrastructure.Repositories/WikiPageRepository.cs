using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Infrastructure.Repositories {
    public class WikiPageRepository : IRepository<WikiPage> {
        private readonly ConcurrentDictionary<string, WikiPage> pages = new ConcurrentDictionary<string,WikiPage>();

        public WikiPage Load(string key) {
            return pages.GetValueOrDefault(key);
        }

        public void Save(WikiPage entity) {
            pages.AddOrUpdate(
                entity.Slug, entity,
                (key, existing) => new WikiPage {
                    Slug = existing.Slug,
                    Text = entity.Text
                }
            );
        }

        public IEnumerable<WikiPage> Query() {
            return pages.Values;
        }
    }
}