using Api.Model.In;
using Api.Model.Out;
using Core.Entities.NameEntry;
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
                GeoLocation = nameEntry.GeoLocation,
                FamousPeople = nameEntry.FamousPeople,
                Media = nameEntry.Media,
                SubmittedBy = nameEntry.CreatedBy,
                Etymology = nameEntry.Etymology,
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
                Etymology = request.Etymology,
                Videos = request.Videos,
                GeoLocation = request.GeoLocation,
                FamousPeople = request.FamousPeople ?? new List<string>(),
                Syllables = request.Syllables ?? new List<string>(),
                Variants = request.Variants ?? new List<string>(),
                CreatedBy = request.SubmittedBy
            };
        }
    }
}
