using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiffMatchPatch;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdater {
        private readonly diff_match_patch patcher;

        public WikiPageUpdater(diff_match_patch patcher) {
            this.patcher = patcher;
        }

        public void Update(WikiPage page, string patchText) {
            var patches = this.patcher.patch_fromText(patchText);
            page.Text = (string)this.patcher.patch_apply(patches, page.Text)[0];
        }
    }
}
