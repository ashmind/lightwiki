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

                this.AllRevisions[value.Number] = revision;
                this.revision = value;
            }
        }

        // this is *not* thread safe
        public IDictionary<int, WikiPageRevision> AllRevisions { get; private set; }

        public override string Text {
            get { return this.Revision.Text; }
            set { throw new NotSupportedException("Set Revision instead."); }
        }

        public override int RevisionNumber {
            get { return this.Revision.Number; }
            set { throw new NotSupportedException("Set Revision instead."); }
        }
    }
}
