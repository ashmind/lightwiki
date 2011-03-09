using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.ViewModels {
    public class MainViewModel {
        public MainViewModel(WikiPageViewModel current, IEnumerable<WikiPage> pages) {
            this.Current = current;
            this.Pages = pages;
        }

        public WikiPageViewModel Current   { get; private set; }
        public IEnumerable<WikiPage> Pages { get; private set; }
    }
}