using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain {
    public class WikiPage {
        public static Func<string, string, WikiPage> Create { get; set; }

        static WikiPage() {
            Create = (slug, text) => new WikiPage(slug) { Text = text };
        }

        protected WikiPage(string slug) {
            this.Slug = slug;
        }

        public string Slug { get; private set; }
        public virtual string Text { get; set; }
    }
}
