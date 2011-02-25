using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.ViewModels {
    public class MainViewModel {
        public MainViewModel(WikiPage currentPage, IList<string> pageSlugs) {
            this.CurrentPage = currentPage;
            this.PageSlugs = pageSlugs;
        }

        public WikiPage CurrentPage { get; private set; }
        public IList<string> PageSlugs { get; private set; }
    }
}