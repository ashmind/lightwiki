using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Domain.Services {
    public class LinkBasedHierarchyProvider : IWikiPageHierarchyProvider {
        private readonly IWikiSyntax syntax;
        private readonly IRepository<WikiPage> repository;

        public LinkBasedHierarchyProvider(
            IWikiSyntax syntax,
            IRepository<WikiPage> repository
        ) {
            this.syntax = syntax;
            this.repository = repository;
        }

        public IEnumerable<WikiPage> GetSubPages(WikiPage page) {
            var links = new SortedSet<string>(this.syntax.GetLinks(page.Text));
            var pages = this.repository.Query().Where(p => links.Contains(p.Slug)).ToDictionary(p => p.Slug);
            return links.Select(link => pages[link]);
        }
    }
}
