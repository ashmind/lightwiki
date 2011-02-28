using System;
using System.Collections.Generic;
using System.Linq;

using DiffMatchPatch;

using MbUnit.Framework;

using AshMind.LightWiki.Domain.Services.Concurrency;

namespace AshMind.LightWiki.Domain.Services.Tests {
    public class WikiPageUpdaterTest {
        [Test]
        [Row("Cat fights fairies", "Cat fights unicorns", "Cat talks with fairies", "Cat talks with unicorns")]
        [Row("ABC", "ABE", "ABD", "ABD")]
        [Row("ABC", "XABC", "ABCY", "XABCY")]
        public void TestConflictResolution(string original, string changeOne, string changeTwo, string expected) {
            var patcher = new diff_match_patch();
            var patchOne = patcher.patch_toText(patcher.patch_make(original, changeOne));
            var patchTwo = patcher.patch_toText(patcher.patch_make(original, changeTwo));

            var page = new VersionedWikiPage("test", original);
            var updater = new WikiPageUpdater(patcher);

            updater.ApplyUpdate(page, 0, patchOne);
            updater.ApplyUpdate(page, 0, patchTwo);

            Assert.AreEqual(expected, page.Text);
        }
    }
}
