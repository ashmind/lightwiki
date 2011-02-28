namespace AshMind.LightWiki.Domain.Services {
    public class WikiPageUpdateDetails {
        public string Patch { get; set; }
        public int ResultRevisionNumber { get; set; }

        public WikiPageUpdateDetails(string patch, int resultRevisionNumber) {
            this.Patch = patch;
            this.ResultRevisionNumber = resultRevisionNumber;
        }
    }
}