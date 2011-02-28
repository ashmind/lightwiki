using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdateResult {
        public WikiPageUpdateResult(
            int revisionNumber,
            string patchForAuthor,
            string patchForOthers
        ) {
            this.RevisionNumber = revisionNumber;
            this.PatchForAuthor = patchForAuthor;
            this.PatchForOthers = patchForOthers;
        }

        public int RevisionNumber { get; private set; }
        public string PatchForAuthor { get; private set; }
        public string PatchForOthers { get; private set; }
    }
}
