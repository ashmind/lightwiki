using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services.Concurrency {
    internal class VersionedWikiPage : WikiPage {
        public VersionedWikiPage(string slug, string text) : base(slug) {
            this.AllRevisions = new ConcurrentDictionary<int, WikiPageRevision>();
            this.Revision = new WikiPageRevision(0, text);
        }

        private WikiPageRevision revision;
        public WikiPageRevision Revision {
            get { return this.revision; }
            set {
                if (this.revision == value)
                    return;

                this.AllRevisions.Add(value.Number, value);
                this.revision = value;
            }
        }

        public override string Text {
            get { return this.Revision.Text; }
            set { throw new NotSupportedException("Set Revision instead."); }
        }

        public IDictionary<int, WikiPageRevision> AllRevisions { get; private set; }
    }
}
