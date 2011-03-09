using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.LightWiki.Domain.Services.Concurrency;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdateResult {
        public WikiPageUpdateResult(
            WikiPageRevision resultingRevision,
            string patchForAuthor,
            string patchForOthers
        ) {
            this.ResultingRevision = resultingRevision;
            this.PatchForAuthor = patchForAuthor;
            this.PatchForOthers = patchForOthers;
        }

        public WikiPageRevision ResultingRevision { get; private set; }
        public string PatchForAuthor { get; private set; }
        public string PatchForOthers { get; private set; }
    }
}
