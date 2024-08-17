﻿using Core.Dto.Request;
using Core.Dto.Response;
using Core.Entities;
using MongoDB.Bson;

namespace Application.Mappers;

public static class SuggestedNameMapper
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
            Name = request.Name.Trim(),
            Email = request.Email?.Trim(),
            Details = request.Details?.Trim(),
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
            GeoLocation = request.GeoLocation.Select(ge => new GeoLocationDto(ge.Id, ge.Place, ge.Region)).ToList(),
        };
    }
}
