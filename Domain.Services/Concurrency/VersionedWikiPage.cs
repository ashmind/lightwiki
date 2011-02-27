using System;
using System.Collections.Generic;
using System.Linq;

using DiffMatchPatch;

namespace AshMind.LightWiki.Domain.Services.Concurrency {
    internal class VersionedWikiPage : WikiPage {
        public VersionedWikiPage(string slug, string text) : base(slug) {
            this.Changes = new Dictionary<int, IList<Patch>>();
            this.Revision = new WikiPageRevision(0, text);
        }

        private WikiPageRevision revision;
        public WikiPageRevision Revision {
            get { return this.revision; }
            set {
                if (this.revision == value)
                    return;

                this.revision = value;
            }
        }

        public override string Text {
            get { return this.Revision.Text; }
            set { throw new NotSupportedException("Set Revision instead."); }
        }

        // this *must* be used only in locked mode!
        public IDictionary<int, IList<Patch>> Changes { get; private set; }

        public override int RevisionNumber {
            get { return this.Revision.Number; }
            set { throw new NotSupportedException("Set Revision instead."); }
        }
    }
}
