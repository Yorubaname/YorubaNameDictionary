using YorubaOrganization.Core.Dto.Request;

namespace Core.Dto.Request;

public record CreateSuggestedNameDto
{  
    public string Name { get; init; }   
    public string Details { get; init; }
    public string Email { get; init; }
    public List<CreateGeoLocationDto> GeoLocation { get; set; }
    public CreateSuggestedNameDto()
    {
        GeoLocation = new List<CreateGeoLocationDto>();
    }

}
