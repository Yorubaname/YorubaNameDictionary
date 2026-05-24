using Words.Core.Dto.Response;

namespace Words.Website.Services
{
    internal static class WordDefinitionFilter
    {
        public static WordEntryDto[] KeepOnlyReviewedDefinitions(WordEntryDto[] words)
        {
            foreach (var word in words)
            {
                var filteredDefinitions = (word.Definitions ?? [])
                    .Where(definition => definition.NeedsReview != true)
                    .ToList();

                word.Definitions = filteredDefinitions;
            }

            return words;
        }
    }
}
