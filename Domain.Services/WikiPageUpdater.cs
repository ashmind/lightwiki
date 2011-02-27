using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.LightWiki.Domain.Services.Concurrency;

using DiffMatchPatch;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdater {
        private readonly diff_match_patch patcher;

        public WikiPageUpdater(diff_match_patch patcher) {
            this.patcher = patcher;
        }

        public WikiPageUpdateResult Update(WikiPage page, int revisionToPatch, string patchText) {
            var versioned = ((VersionedWikiPage)page);
            var revision = versioned.AllRevisions[revisionToPatch];
            var patches = this.patcher.patch_fromText(patchText);
            
            var text = (string)this.patcher.patch_apply(patches, revision.Text)[0];
            var newPatches = (IList<Patch>)null;
            var revisionNumber = 0;
            lock (versioned) {
                newPatches = LockedUpdate(versioned, revision, text);
                revisionNumber = versioned.Revision.Number;
            }

            return new WikiPageUpdateResult(
                revisionNumber,
                this.patcher.patch_toText(newPatches)
            );
        }

        private IList<Patch> LockedUpdate(VersionedWikiPage page, WikiPageRevision revision, string newTextForRevision) {
            if (page.Revision == revision) {
                // patching current revision, everything is easy
                page.Revision = new WikiPageRevision(revision.Number + 1, newTextForRevision);
                return new Patch[0];
            }

            // patching old revision, everything is harder
            var newerPatches = this.patcher.patch_make(newTextForRevision, page.Text);
            page.Revision = new WikiPageRevision(
                page.Revision.Number + 1,
                (string)this.patcher.patch_apply(newerPatches, newTextForRevision)[0]
            );

            return newerPatches;
        }
    }
}