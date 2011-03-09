using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.ViewModels {
    public class WikiPageViewModel {
        public WikiPageViewModel(WikiPage page, string html) {
            this.Html = html;
            this.Page = page;
        }

        public WikiPage Page { get; private set; }

        public string Html { get; private set; }
        public string Text {
            get { return this.Page.Text; }
        }
    }
}