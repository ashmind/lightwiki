using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services.Syntax {
    public interface IWikiOutputFormat {
        string Escape(string value);

        string MakeBold(string value);
        string MakeItalic(string value);
        string MakeStrikeThrough(string value);
        string MakeLink(string value);

        string EndOfLine { get; }
    }
}
