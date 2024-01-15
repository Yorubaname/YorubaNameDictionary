using Api.Model.In;
using Api.Model.Out;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;

namespace Api.Mappers
{
    public static class NamesControllerMappers
    {
        public static NameEntryDto[] MapToDtoCollection(this List<NameEntry> names)
        {
            return names.Select(nameEntry => new NameEntryDto
            {
                Pronunciation = nameEntry.Pronunciation,
                IpaNotation = nameEntry.IpaNotation,
                Variants = nameEntry.Variants,
                Syllables = nameEntry.Syllables,
                Meaning = nameEntry.Meaning,
                ExtendedMeaning = nameEntry.ExtendedMeaning,
                Morphology = nameEntry.Morphology,
                GeoLocation = nameEntry.GeoLocation.Select(ge => new GeoLocationDto(ge.Place, ge.Region)).ToList(),
                FamousPeople = nameEntry.FamousPeople,
                Media = nameEntry.Media,
                SubmittedBy = nameEntry.CreatedBy,
                Etymology = nameEntry.Etymology.Select(et => new EtymologyDto(et.Part, et.Meaning)).ToList(),
                State = nameEntry.State,
                CreatedAt = nameEntry.CreatedAt,
                UpdatedAt = nameEntry.UpdatedAt,
                Name = nameEntry.Name
            }).ToArray();
        }

        public static NameEntry MapToEntity(this CreateNameDto request)
        {
            return new NameEntry
            {
                Name = request.Name,
                Pronunciation = request.Pronunciation?.Trim(),
                Meaning = request.Meaning.Trim(),
                ExtendedMeaning = request.ExtendedMeaning?.Trim(),
                Morphology = request.Morphology ?? new List<string>(),
                Media = request.Media ?? new List<string>(),
                State = request.State ?? State.NEW,
                Etymology = request.Etymology.Select(et => new Etymology(et.Part, et.Meaning)).ToList(),
                Videos = request.Videos.Select(ev => new EmbeddedVideo(ev.VideoId, ev.Caption)).ToList(),
                // TODO: Add validation for these values to ensure illegal values are not entered
                GeoLocation = request.GeoLocation.Select(ge => new GeoLocation(ge.Place, ge.Region)).ToList(),
                FamousPeople = request.FamousPeople ?? new List<string>(),
                Syllables = request.Syllables ?? new List<string>(),
                Variants = request.Variants ?? new List<string>(),
                CreatedBy = request.SubmittedBy
            };
        }
    }
}
