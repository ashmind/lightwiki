using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AshMind.LightWiki.Web.Urls {
    public class SimpleWikiUrlProvider : IWikiUrlProvider {
        private readonly string applicationPath;

        public SimpleWikiUrlProvider(string applicationPath) {
            this.applicationPath = applicationPath;
        }
        
        public string GetUrl(string slug) {
            return this.applicationPath + "/wiki/" + slug;
        }
    }
}
