using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Entities.Partials;
using YorubaOrganization.Core.Enums;

namespace Application.Mappers
{
    public static class NameEntryMapper
    {
        public static NameEntryDto[] MapToDtoCollection(this IEnumerable<NameEntry> names)
        {
            return names.Select(nameEntry => nameEntry.MapToDto()).ToArray();
        }

        public static NameEntryMiniDto[] MapToDtoCollectionMini(this IEnumerable<NameEntry> names)
        {
            return names.Select(nameEntry => new NameEntryMiniDto
            {
                Name = nameEntry.Title,
                Meaning = nameEntry.Meaning,
                SubmittedBy = nameEntry.CreatedBy
            }).ToArray();
        }

        public static NameEntry MapToEntity(this NameDto request)
        {
            return new NameEntry
            {
                Title = request.Name.Trim(),
                Pronunciation = request.Pronunciation?.Trim(),
                Meaning = request.Meaning.Trim(),
                ExtendedMeaning = request.ExtendedMeaning?.Trim(),
                Morphology = request.Morphology ?? new List<string>(),
                MediaLinks = request.MediaLinks,
                State = request.State ?? State.NEW,
                Etymology = request.Etymology.Select(et => new Etymology(et.Part, et.Meaning)).ToList(),
                Videos = request.Videos.Select(ev => new EmbeddedVideo(ev.VideoId, ev.Caption)).ToList(),
                // TODO Later: Add validation for these values to ensure illegal values are not entered
                GeoLocation = request.GeoLocation.Select(ge => new GeoLocation(ge.Place, ge.Region)).ToList(),
                FamousPeople = request.FamousPeople ?? new List<string>(),
                Syllables = request.Syllables ?? new List<string>(),
                VariantsV2 = request.VariantsV2,
                CreatedBy = request.SubmittedBy,
                UpdatedBy = request.SubmittedBy
            };
        }

        public static NameEntryDto MapToDto(this NameEntry nameEntry)
        {
            return new NameEntryDto
            {
                Pronunciation = nameEntry.Pronunciation,
                IpaNotation = nameEntry.IpaNotation,
                Variants = (CommaSeparatedString)nameEntry.VariantsV2.Select(v => v.Title).ToList(),
                Syllables = (HyphenSeparatedString)nameEntry.Syllables,
                Meaning = nameEntry.Meaning,
                ExtendedMeaning = nameEntry.ExtendedMeaning,
                Morphology = (CommaSeparatedString)nameEntry.Morphology,
                GeoLocation = nameEntry.GeoLocation.Select(ge => new GeoLocationDto(ge.Id, ge.Place, ge.Region)).ToList(),
                FamousPeople = (CommaSeparatedString)nameEntry.FamousPeople,
                Media = (CommaSeparatedString)nameEntry.MediaLinks.Select(m => m.Url).ToList(),
                SubmittedBy = nameEntry.CreatedBy,
                Etymology = nameEntry.Etymology.Select(et => new EtymologyDto(et.Part, et.Meaning)).ToList(),
                Videos = nameEntry.Videos.Select(v => new EmbeddedVideoDto(v.VideoId, v.Caption)).ToList(),
                State = nameEntry.State,
                CreatedAt = nameEntry.CreatedAt,
                UpdatedAt = nameEntry.UpdatedAt,
                Name = nameEntry.Title
            };
        }
    }
}
