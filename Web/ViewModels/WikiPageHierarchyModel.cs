using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
 
using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.ViewModels {
    public class WikiPageHierarchyModel {
        private readonly Func<WikiPage, IEnumerable<WikiPage>> getChildren;

        public WikiPageHierarchyModel(IEnumerable<WikiPage> pages, Func<WikiPage, IEnumerable<WikiPage>> getChildren) {
            this.Pages = pages.ToList().AsReadOnly();
            this.getChildren = getChildren;
        }

        public ReadOnlyCollection<WikiPage> Pages { get; private set; }

        public WikiPageHierarchyModel GetNextLevel(WikiPage page) {
            return new WikiPageHierarchyModel(this.getChildren(page), this.getChildren);
        }
    }
}