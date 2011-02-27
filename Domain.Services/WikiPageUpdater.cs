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
            var patches = this.patcher.patch_fromText(patchText);
            
            var revisionPatches = (IList<Patch>)null;
            var revisionNumber = 0;
            lock (versioned) {
                LockedUpdate(versioned, revisionToPatch, patches);
                revisionNumber = versioned.Revision.Number;
                revisionPatches = versioned.Changes[revisionNumber];
            }

            return new WikiPageUpdateResult(
                revisionNumber,
                "",
                this.patcher.patch_toText(revisionPatches)
            );
        }

        private void LockedUpdate(VersionedWikiPage page, int revisionToPatch, List<Patch> patches) {
            /*if (revisionToPatch != page.Revision.Number)
                this.LockedMergePatches(page, revisionToPatch, patches);*/

            LockedPatchCurrentRevision(page, patches, regeneratePatches : revisionToPatch != page.Revision.Number);
        }

        // simple situation
        private void LockedPatchCurrentRevision(VersionedWikiPage page, List<Patch> patches, bool regeneratePatches) {
            var previousText = page.Text;
            page.Revision = new WikiPageRevision(
                page.RevisionNumber + 1,
                (string)this.patcher.patch_apply(patches, previousText)[0]
            );
            if (regeneratePatches)
                patches = this.patcher.patch_make(previousText, page.Text);

            page.Changes.Add(page.RevisionNumber, patches);
        }

        // complex situation
        /*private void LockedMergePatches(VersionedWikiPage page, int revisionToPatch, IList<Patch> patches) {
            for (var i = revisionToPatch + 1; i <= page.Revision.Number; i++) {
                this.Merge(patches, page.Changes[i]);
            }
        }

        private void Merge(IList<Patch> patches, IList<Patch> precedingPatches) {
            for (var i = 0; i < patches.Count; i++) {
                var patch = patches[i];
                foreach (var preceding in precedingPatches) {
                    if (preceding.start1 > patch.start1 + patch.length1)
                        break;

                    bool keep;
                    this.MergeActuallyPreceding(patch, preceding, out keep);

                    if (!keep) {
                        patches.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
        }

        private void MergeActuallyPreceding(Patch patch, Patch preceding, out bool keep) {
            if (preceding.start1 + preceding.length1 >= patch.start1 + patch.length1) {
                keep = false;
                return;
            }

            patch.start1 += preceding.length2 - preceding.length1;
            patch.start2 += preceding.length2 - preceding.length1;
            keep = true;
        }*/
    }
}