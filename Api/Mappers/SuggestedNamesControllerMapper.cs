using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using Core.Entities.NameEntry;
using MongoDB.Bson;

namespace Api.Mappers;

public static class SuggestedNamesControllerMapper
{
    public static SuggestedNameDto[] MapToDtoCollection(this IEnumerable<SuggestedName> names)
    {
        return names.Select(nameEntry => MapToDto(nameEntry)).ToArray();
    }
    public static SuggestedName MapToEntity(this CreateSuggestedNameDto request)
    {
        return new SuggestedName
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = request.Name,
            Email = request.Email,
            Details = request.Details,
            GeoLocation = request.GeoLocation.Select(x => new GeoLocation
            {
                Place = x.Place,
                Region = x.Region,
                Id = ObjectId.GenerateNewId().ToString()
            }).ToList(),
        };
    }

    public static SuggestedNameDto MapToDto(this SuggestedName request)
    {
      
        return new SuggestedNameDto
        {
            Id = request.Id,
            Name = request.Name,
            Email = request.Email,
            Details = request.Details,
            GeoLocation = request.GeoLocation.Select(ge => new GeoLocationDto(ge.Place, ge.Region)).ToList(),
        };
    }
}
