using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.LightWiki.Domain.Services.Concurrency;

using DiffMatchPatch;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdater {
        #region PatchSets

        private class PatchSets {
            public bool AcceptForAuthor { get; private set; }
            public IList<Patch> PatchesForAuthor { get; private set; }
            public IList<Patch> PatchesForOthers { get; private set; }

            public PatchSets(bool acceptForAuthor, IList<Patch> patchesForAuthor, IList<Patch> patchesForOthers) {
                this.AcceptForAuthor = acceptForAuthor;
                this.PatchesForAuthor = patchesForAuthor;
                this.PatchesForOthers = patchesForOthers;
            }
        }

        #endregion

        private readonly diff_match_patch patcher;

        public WikiPageUpdater(diff_match_patch patcher) {
            this.patcher = patcher;
        }

        public WikiPageUpdateResult Update(WikiPage page, int revisionToPatch, string patchText) {
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
                patchSets.AcceptForAuthor,
                this.patcher.patch_toText(patchSets.PatchesForAuthor),
                this.patcher.patch_toText(patchSets.PatchesForOthers)
            );
        }

        private PatchSets LockedUpdate(VersionedWikiPage page, int revisionToPatch, List<Patch> patches) {
            var patchingCurrentRevision = revisionToPatch == page.RevisionNumber;

            var previousText = page.Text;
            page.Revision = new WikiPageRevision(
                page.RevisionNumber + 1,
                (string)this.patcher.patch_apply(patches, previousText)[0]
            );
            if (patchingCurrentRevision) {
                return new PatchSets(
                    acceptForAuthor: true,
                    patchesForAuthor: new Patch[0],
                    patchesForOthers: patches
                );
            }
            else {
                return new PatchSets(
                    acceptForAuthor: false,
                    patchesForAuthor: this.patcher.patch_make(page.AllRevisions[revisionToPatch].Text, page.Text),
                    patchesForOthers: this.patcher.patch_make(previousText, page.Text)
                );
            }
        }
    }
}