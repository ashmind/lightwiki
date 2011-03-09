using System;
using System.Collections.Generic;
using System.Linq;
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

        public string EndOfLine {
            get { return "<br />"; }
        }
    }
}