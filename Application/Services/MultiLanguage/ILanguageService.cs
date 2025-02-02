namespace Application.Services.MultiLanguage
{
    public interface ILanguageService
    {
        string Website { get; }

        /// <summary>
        /// The language of the records in the dictionary, in a displayable format.
        /// </summary>
        string LanguageDisplay { get; }

        string SocialName { get; }

        bool IsYoruba { get; }
        bool IsIgbo { get; }
        string CurrentTenant { get; }
    }
}
