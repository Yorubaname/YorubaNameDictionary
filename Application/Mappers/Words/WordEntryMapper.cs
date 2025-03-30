using Words.Core.Dto.Response;
using Words.Core.Entities;
using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;

namespace Application.Mappers.Words
{
    public static class WordEntryMapper
    {
        public static WordEntryDto MapToDto(this WordEntry wordEntry)
        {
            return new WordEntryDto
            {
                Word = wordEntry.Title,
                Pronunciation = wordEntry.Pronunciation,
                PartOfSpeech = wordEntry.PartOfSpeech.ToString(),
                Style = wordEntry.Style?.ToString(),
                GrammaticalFeature = wordEntry.GrammaticalFeature?.ToString(),
                IpaNotation = wordEntry.IpaNotation,
                Variants = [.. wordEntry.VariantsV2.Select(v => new VariantDto(v.Title, v.GeoLocation?.Place))],
                Syllables = (HyphenSeparatedString)wordEntry.Syllables,
                Morphology = (CommaSeparatedString)wordEntry.Morphology,

                GeoLocation = [.. wordEntry.GeoLocation.Select(ge => new GeoLocationDto(ge.Id, ge.Place, ge.Region))],
                Etymology = [.. wordEntry.Etymology.Select(et => new EtymologyDto(et.Part, et.Meaning))],
                MediaLinks = [.. wordEntry.MediaLinks.Select(m => new MediaLinkDto(m.Url, m.Description, m.Type))],
                Definitions = [.. wordEntry.Definitions.Select(d => d.MapToDto())],

                State = wordEntry.State,

                SubmittedBy = wordEntry.CreatedBy,
                CreatedAt = wordEntry.CreatedAt,
                UpdatedAt = wordEntry.UpdatedAt
            };
        }

        public static DefinitionDto MapToDto(this Definition definition)
        {
            return new DefinitionDto(
                definition.Content,
                definition.EnglishTranslation,
                [.. definition
                        .Examples
                        .Select(example =>
                        new DefinitionExampleDto(example.Content, example.EnglishTranslation, example.Type)
                        )],
                definition.CreatedAt
            );
        }

        public static WordEntryDto[] MapToDtoCollection(this IEnumerable<WordEntry> wordEntries)
        {
            return [.. wordEntries.Select(wordEntry => wordEntry.MapToDto())];
        }
    }
}
