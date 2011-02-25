using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AshMind.Extensions;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction;

namespace AshMind.LightWiki.Infrastructure.Repositories {
    public class WikiPageRepository : IRepository<WikiPage>, IDisposable {
        private readonly ILocation root;
        private readonly ConcurrentDictionary<string, WikiPage> pages = new ConcurrentDictionary<string, WikiPage>();
        private IDictionary<string, WikiPage> previousSnapshot;

        private readonly Timer timer;

        public WikiPageRepository(ILocation root) {
            this.root = root;
            this.LoadFromFiles();
            timer = new Timer(_ => this.SaveToFiles(), null, 1000, 1000);
        }

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

        private void LoadFromFiles() {
            foreach (var file in root.GetFiles()) {
                var slug = file.NameWithoutExtension;
                pages.TryAdd(slug, new WikiPage {
                    Slug = slug,
                    Text = file.ReadAllText()
                });
            }

            this.previousSnapshot = this.GetSnapshot();
        }

        private void SaveToFiles() {
            var snapshot = this.GetSnapshot();
            foreach (var page in snapshot.Values) {
                var previous = previousSnapshot.GetValueOrDefault(page.Slug);
                if (previous != null && previous.Text == page.Text)
                    continue;

                var file = this.root.GetFile(page.Slug + ".wiki", false);
                file.WriteAllText(page.Text);
            }
            this.previousSnapshot = snapshot;
        }

        private IDictionary<string, WikiPage> GetSnapshot() {
            return pages.ToDictionary(
                pair => pair.Key,
                pair => new WikiPage {
                    Slug = pair.Key,
                    Text = pair.Value.Text
                }
            );
        }

        #region IDisposable Members

        public void Dispose() {
            this.timer.Dispose();
        }

        #endregion
    }
}