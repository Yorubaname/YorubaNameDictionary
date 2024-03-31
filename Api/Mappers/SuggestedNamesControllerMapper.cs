using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using MongoDB.Bson;

namespace Api.Mappers;

public static class SuggestedNamesControllerMapper
{
    public static SuggestedName MapToEntity(this CreateSuggestedNameDto request)
    {
        return new SuggestedName
        {
            //Id = ObjectId.GenerateNewId().ToString(),
            Name = request.Name,
            Email = request.Email,
            Details = request.Details,
            //GeoLocation = request.GeoLocation.Select(x => new GeoLocation
            //{
            //    Id = ObjectId.GenerateNewId().ToString(),
            //    Place = x.Place,
            //    Region = x.Region
            //}).ToList()

            GeoLocation = request.GeoLocation.Select(ge => new GeoLocation(ge.Place, ge.Region)).ToList(),
        };
    }

    public static SuggestedNameDto MapToDto(this SuggestedName request)
    {
        return new SuggestedNameDto
        {
            Name = request.Name,
            Email = request.Email,
            Details = request.Details,
            GeoLocation = request.GeoLocation.Select(ge => new GeoLocationDto(ge.Place, ge.Region)).ToList(),
        };
    }
}
