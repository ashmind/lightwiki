using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdateResult {
        public WikiPageUpdateResult(int revisionNumber, string patchToRevision) {
            this.RevisionNumber = revisionNumber;
            this.PatchToRevision = patchToRevision;
        }

        public int RevisionNumber { get; private set; }
        public string PatchToRevision { get; private set; }
    }
}
