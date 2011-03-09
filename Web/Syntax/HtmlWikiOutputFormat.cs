using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using AshMind.LightWiki.Domain.Services.Syntax;

namespace AshMind.LightWiki.Web.Syntax {
    public class HtmlWikiOutputFormat : IWikiOutputFormat {
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
            return string.Format(
                "<a href='{0}'>{1}</a>",
                Regex.Replace(value.Trim(), @"\s+", "_"),
                value
            );
        }

        public string EndOfLine {
            get { return "<br />"; }
        }
    }
}