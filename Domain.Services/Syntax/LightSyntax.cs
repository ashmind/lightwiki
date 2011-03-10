using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AshMind.LightWiki.Domain.Services.Syntax {
    public class LightSyntax : IWikiSyntax {
        public string Convert(string markup, IWikiOutputFormat format) {
            var result = format.Escape(markup);

            result = SimpleTransform(result, "*", "*",  format.MakeBold);
            result = SimpleTransform(result, "_", "_",  format.MakeItalic);
            result = SimpleTransform(result, "-", "-",  format.MakeStrikeThrough);
            result = SimpleTransform(result, "[", "]",  format.MakeLink);

            result = Regex.Replace(result, @"\r\n?|\n", format.EndOfLine);

            return result;
        }

        private string SimpleTransform(string markup, string start, string end, Func<string, string> transform) {
            return this.GetSimpleRegex(start, end)
                       .Replace(markup, match => transform(match.Groups[1].Value));
        }

        public IEnumerable<string> GetLinks(string text) {
            return this.GetSimpleRegex("[", "]")
                       .Matches(text).Cast<Match>()
                       .Select(m => m.Groups[1].Value);
        }

        private Regex GetSimpleRegex(string start, string end) {
            return new Regex(
                Regex.Escape(start) + @"(\w+)" + Regex.Escape(end)
            );
        }
    }
}
