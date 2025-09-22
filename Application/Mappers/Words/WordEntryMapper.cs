using Core.Dto.Response;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Entities.Partials;

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
                Variants = [.. wordEntry.VariantsV2.Select(v => new VariantDto(v.Title,
                                v.GeoLocation == null ?
                                null :
                                new GeoLocationDto(v.GeoLocation.Id, v.GeoLocation.Place, v.GeoLocation.Region)))],
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

        public static WordEntry MapToEntity(this WordDto dto)
        {
            var wordEntry = new WordEntry
            {
                Title = dto.Word,
                Pronunciation = dto.Pronunciation,
                PartOfSpeech = dto.PartOfSpeech,
                Style = dto.Style,
                GrammaticalFeature = dto.GrammaticalFeature,
                IpaNotation = dto.IpaNotation,
                VariantsV2 = dto.Variants?.Select(v =>
                new Variant(v.Word, v.Geolocation is not null ? new GeoLocation { Place = v.Geolocation.Place, Region = v.Geolocation.Region } : null)).ToList() ?? [],
                Syllables = dto.Syllables ?? new List<string>(),
                Morphology = dto.Morphology ?? new List<string>(),
                GeoLocation = dto.GeoLocation?.Select(g => new GeoLocation(g.Place, g.Region)).ToList() ?? [],
                Etymology = dto.Etymology?.Select(e => new Etymology(e.Part, e.Meaning)).ToList() ?? [],
                MediaLinks = dto.MediaLinks?.Select(m => new MediaLink(m.Link, m.Caption, m.Type)).ToList() ?? [],
                Definitions = dto.Definitions?.Select(d => new Definition
                {
                    Content = d.Content,
                    EnglishTranslation = d.EnglishTranslation,
                    Examples = d.Examples?.Select(ex => new DefinitionExample
                    {
                        Content = ex.Content,
                        EnglishTranslation = ex.EnglishTranslation,
                        Type = ex.Type
                    }).ToList() ?? []
                }).ToList() ?? [],
                State = dto.State ?? YorubaOrganization.Core.Enums.State.NEW,
                CreatedBy = dto.SubmittedBy,
                UpdatedBy = dto.SubmittedBy
            };

            return wordEntry;
        }


        public static WordEntryDto[] MapToDtoCollection(this IEnumerable<WordEntry> wordEntries)
        {
            return [.. wordEntries.Select(wordEntry => wordEntry.MapToDto())];
        }

        public static WordEntryMiniDto[] MapToDtoCollectionMini(this IEnumerable<WordEntry> words)
        {
            return [.. words.Select(w => new WordEntryMiniDto
            {
                Word = w.Title,
                Definitions = [.. w.Definitions
                                    .Select(d => new DefinitionDto
                                    (
                                        d.Content,
                                        d.EnglishTranslation,
                                        [.. d.Examples.Select(ex => new DefinitionExampleDto(ex.Content, ex.EnglishTranslation, ex.Type))],
                                        d.CreatedAt)
                                    )],
            })];
        }
    }
}
