using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.ViewModels {
    public class MainViewModel {
        public MainViewModel(WikiPage currentPage, IEnumerable<WikiPage> pages) {
            this.CurrentPage = currentPage;
            this.Pages = pages;
        }

        public WikiPage CurrentPage         { get; private set; }
        public IEnumerable<WikiPage> Pages  { get; private set; }
    }
}