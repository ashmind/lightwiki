using AshMind.LightWiki.Domain.Services.Concurrency;

namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdateDetails {
        public string Patch { get; set; }
        public WikiPageRevision ResultingRevision { get; set; }

        public WikiPageUpdateDetails(string patch, WikiPageRevision resultingRevision) {
            this.Patch = patch;
            this.ResultingRevision = resultingRevision;
        }
    }
}