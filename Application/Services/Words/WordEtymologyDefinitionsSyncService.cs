using Microsoft.Extensions.Logging;

namespace Application.Services.Words
{
    public record WordEtymologyDefinitionsSyncPageResult(
        int Page,
        int WordsRead,
        int EtymologyRowsRead,
        int DistinctParts,
        int DistinctPartMeaningPairs,
        int CreatedWords,
        int DefinitionsAdded,
        int SkippedDuplicates,
        int PartiallyCreated,
        int NoValidDefinitions,
        IDictionary<string, string> StatusByWord);

    public class WordEtymologyDefinitionsSyncService(
        WordEntryService wordEntryService,
        ILogger<WordEtymologyDefinitionsSyncService> logger)
    {
        private readonly WordEntryService _wordEntryService = wordEntryService;
        private readonly ILogger<WordEtymologyDefinitionsSyncService> _logger = logger;

        public async Task<WordEtymologyDefinitionsSyncPageResult> ProcessPageAsync(
            int page,
            int pageSize,
            string runId,
            int wordLogSampleSize,
            string currentUser = WordEntryService.SystemUser)
        {
            var words = await _wordEntryService.GetPublishedWithEtymologyPageAsync(page, pageSize);

            var flattenedEtymology = words
                .SelectMany(word => word.Etymology ?? [])
                .Where(et => !string.IsNullOrWhiteSpace(et.Part) && !string.IsNullOrWhiteSpace(et.Meaning))
                .Select(et => new
                {
                    Part = et.Part.Trim(),
                    Meaning = et.Meaning.Trim()
                })
                .ToList();

            var groupedByPart = flattenedEtymology
                .GroupBy(et => et.Part, StringComparer.CurrentCultureIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(et => et.Meaning)
                        .Distinct(StringComparer.CurrentCultureIgnoreCase)
                        .ToArray(),
                    StringComparer.CurrentCultureIgnoreCase);

            var statusByWord = groupedByPart.Count == 0
                ? new Dictionary<string, string>()
                : await _wordEntryService.AddEnglishDefinitionsAsync(groupedByPart, currentUser);

            var createdWords = statusByWord.Values.Count(status => status == "created");
            var definitionsAdded = statusByWord.Values.Count(status => status == "all-definitions-added");
            var skippedDuplicates = statusByWord.Values.Count(status => status == "skipped-duplicate");
            var partiallyCreated = statusByWord.Values.Count(status => status == "some-definitions-added");
            var noValidDefinitions = statusByWord.Values.Count(status => status == "no-valid-definition");

            foreach (var sampledWord in statusByWord
                .OrderBy(kvp => kvp.Key, StringComparer.CurrentCultureIgnoreCase)
                .Take(Math.Max(0, wordLogSampleSize)))
            {
                _logger.LogInformation(
                    "WordEtymologyDefinitionsSync.WordResult runId={RunId} page={Page} word={Word} status={Status}",
                    runId,
                    page,
                    sampledWord.Key,
                    sampledWord.Value);
            }

            return new WordEtymologyDefinitionsSyncPageResult(
                page,
                words.Count,
                flattenedEtymology.Count,
                groupedByPart.Count,
                groupedByPart.Values.Sum(values => values.Length),
                createdWords,
                definitionsAdded,
                skippedDuplicates,
                partiallyCreated,
                noValidDefinitions,
                statusByWord);
        }
    }
}