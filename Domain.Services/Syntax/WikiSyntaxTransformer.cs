﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AshMind.LightWiki.Domain.Services.Syntax {
    public class WikiSyntaxTransformer {
        public string Transform(string markup, IWikiOutputFormat format) {
            var result = format.Escape(markup);

            result = SimpleTransform(result, @"\*", @"\*",  format.MakeBold);
            result = SimpleTransform(result, "_", "_",      format.MakeItalic);
            result = SimpleTransform(result, "-", "-",      format.MakeStrikeThrough);

            result = Regex.Replace(result, @"\r\n", format.EndOfLine);

            return result;
        }

        private string SimpleTransform(string markup, string start, string end, Func<string, string> transform) {
            return Regex.Replace(
                markup, start + @"(\w+)" + end, match => transform(match.Groups[1].Value)
            );
        }
    }
}
