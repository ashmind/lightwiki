using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services {
    public interface IWikiPageHierarchyProvider {
        IEnumerable<WikiPage> GetSubPages(WikiPage page);
    }
}
