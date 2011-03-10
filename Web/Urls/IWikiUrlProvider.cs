using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.LightWiki.Web.Urls {
    public interface IWikiUrlProvider {
        string GetUrl(string slug);
    }
}
