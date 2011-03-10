using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Web.Urls;

namespace AshMind.LightWiki.Web.Syntax {
    public class HtmlWikiOutputFormat : IWikiOutputFormat {
        private readonly IRepository<WikiPage> pageRepository;
        private readonly IWikiUrlProvider urlProvider;

        public HtmlWikiOutputFormat(IRepository<WikiPage> pageRepository, IWikiUrlProvider urlProvider) {
            this.pageRepository = pageRepository;
            this.urlProvider = urlProvider;
        }

        public string Escape(string value) {
            return HttpUtility.HtmlEncode(value);
        }

        public string MakeBold(string value) {
            return "<b>" + value + "</b>";
        }

        public string MakeItalic(string value) {
            return "<i>" + value + "</i>";
        }

        public string MakeStrikeThrough(string value) {
            return "<s>" + value + "</s>";
        }

        public string MakeLink(string value) {
            var slug = Regex.Replace(value.Trim(), @"\s+", "_");
            var @class = !pageRepository.Query().Any(p => p.Slug == slug) ? "new" : "";

            return string.Format(
                "<a class='wiki {0}' href='{1}'>{2}</a>",
                @class, this.urlProvider.GetUrl(slug), value
            );
        }

        public string EndOfLine {
            get { return "<br />"; }
        }
    }
}