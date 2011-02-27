using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Domain.Services.Concurrency {
    public class WikiPageRevision {
        public WikiPageRevision(int number, string text) {
            this.Number = number;
            this.Text = text;
        }

        public int Number { get; private set; }
        public string Text { get; private set; }
    }
}
