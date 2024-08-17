using Core.Dto;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;

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
                Name = nameEntry.Name,
                Meaning = nameEntry.Meaning,
                SubmittedBy = nameEntry.CreatedBy
            }).ToArray();
        }

        public static NameEntry MapToEntity(this NameDto request)
        {
            return new NameEntry
            {
                Name = request.Name.Trim(),
                Pronunciation = request.Pronunciation?.Trim(),
                Meaning = request.Meaning.Trim(),
                ExtendedMeaning = request.ExtendedMeaning?.Trim(),
                Morphology = request.Morphology ?? new List<string>(),
                Media = request.Media ?? new List<string>(),
                State = request.State ?? State.NEW,
                Etymology = request.Etymology.Select(et => new Etymology(et.Part, et.Meaning)).ToList(),
                Videos = request.Videos.Select(ev => new EmbeddedVideo(ev.VideoId, ev.Caption)).ToList(),
                // TODO Later: Add validation for these values to ensure illegal values are not entered
                GeoLocation = request.GeoLocation.Select(ge => new GeoLocation(ge.Place, ge.Region)).ToList(),
                FamousPeople = request.FamousPeople ?? new List<string>(),
                Syllables = request.Syllables ?? new List<string>(),
                Variants = request.Variants ?? new List<string>(),
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
                Variants = (CommaSeparatedString)nameEntry.Variants,
                Syllables = (HyphenSeparatedString)nameEntry.Syllables,
                Meaning = nameEntry.Meaning,
                ExtendedMeaning = nameEntry.ExtendedMeaning,
                Morphology = (CommaSeparatedString)nameEntry.Morphology,
                GeoLocation = nameEntry.GeoLocation.Select(ge => new GeoLocationDto(ge.Id, ge.Place, ge.Region)).ToList(),
                FamousPeople = (CommaSeparatedString)nameEntry.FamousPeople,
                Media = (CommaSeparatedString)nameEntry.Media,
                SubmittedBy = nameEntry.CreatedBy,
                Etymology = nameEntry.Etymology.Select(et => new EtymologyDto(et.Part, et.Meaning)).ToList(),
                Videos = nameEntry.Videos.Select(v => new EmbeddedVideoDto(v.VideoId, v.Caption)).ToList(),
                State = nameEntry.State,
                CreatedAt = nameEntry.CreatedAt,
                UpdatedAt = nameEntry.UpdatedAt,
                Name = nameEntry.Name
            };
        }
    }
}
