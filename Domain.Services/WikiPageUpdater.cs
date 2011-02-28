using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AshMind.LightWiki.Domain.Services.Concurrency;

using DiffMatchPatch;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdater {
        #region PatchSets

        private class PatchSets {
            public IList<Patch> PatchesForAuthor { get; private set; }
            public IList<Patch> PatchesForOthers { get; private set; }

            public PatchSets(IList<Patch> patchesForAuthor, IList<Patch> patchesForOthers) {
                this.PatchesForAuthor = patchesForAuthor;
                this.PatchesForOthers = patchesForOthers;
            }
        }

        #endregion

        private readonly diff_match_patch patcher;

        public WikiPageUpdater(diff_match_patch patcher) {
            this.patcher = patcher;
        }

        public WikiPageUpdateResult ApplyUpdate(WikiPage page, int revisionToPatch, string patchText) {
            var versioned = ((VersionedWikiPage)page);
            var patches = this.patcher.patch_fromText(patchText);
            
            var patchSets = (PatchSets)null;
            var revisionNumber = 0;
            lock (versioned) {
                patchSets = LockedUpdate(versioned, revisionToPatch, patches);
                revisionNumber = versioned.Revision.Number;
            }
            
            return new WikiPageUpdateResult(
                revisionNumber,
                this.patcher.patch_toText(patchSets.PatchesForAuthor),
                this.patcher.patch_toText(patchSets.PatchesForOthers)
            );
        }

        private PatchSets LockedUpdate(VersionedWikiPage page, int revisionToPatch, List<Patch> patches) {
            var patchingCurrentRevision = revisionToPatch == page.RevisionNumber;

            var previousRevision = page.Revision;
            var newText = (string)this.patcher.patch_apply(patches, previousRevision.Text)[0];
            if (newText != previousRevision.Text)
                page.Revision = new WikiPageRevision(page.RevisionNumber + 1, newText);

            if (patchingCurrentRevision) {
                Debug.WriteLine(string.Format("Accepted {0}=>{1}.", revisionToPatch, page.RevisionNumber));
                return new PatchSets(
                    patchesForAuthor: new Patch[0],
                    patchesForOthers: patches
                );
            }
            else {
                Debug.WriteLine(string.Format(
                    "Upgraded {0}=>{1} to {2}=>{3}.",
                    revisionToPatch, revisionToPatch + 1, previousRevision.Number, page.RevisionNumber
                ));
                return new PatchSets(
                    patchesForAuthor: this.patcher.patch_make(page.AllRevisions[revisionToPatch].Text, page.Text),
                    patchesForOthers: this.patcher.patch_make(previousRevision.Text, page.Text)
                );
            }
        }

        public WikiPageUpdateDetails GetUpdate(WikiPage page, int fromRevision) {
            var versioned = ((VersionedWikiPage)page);
            var currentRevision = versioned.Revision;

            var patch = this.patcher.patch_make(
                versioned.AllRevisions[fromRevision].Text,
                currentRevision.Text
            );
            return new WikiPageUpdateDetails(
                this.patcher.patch_toText(patch),
                currentRevision.Number
            );
        }
    }
}