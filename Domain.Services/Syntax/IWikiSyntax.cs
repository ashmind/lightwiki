using System.Collections.Generic;

namespace AshMind.LightWiki.Domain.Services.Syntax {
    public interface IWikiSyntax {
        string Convert(string markup, IWikiOutputFormat format);
        IEnumerable<string> GetLinks(string text);
    }
}