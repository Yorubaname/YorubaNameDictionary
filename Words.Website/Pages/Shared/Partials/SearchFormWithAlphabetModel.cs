namespace Words.Website.Pages.Shared.Partials
{
    public class SearchFormWithAlphabetModel
    {
        public bool ShowTagline { get; set; }
        public bool ShowAlphabet { get; set; }
        public IEnumerable<string>? Letters { get; set; }
    }
}
