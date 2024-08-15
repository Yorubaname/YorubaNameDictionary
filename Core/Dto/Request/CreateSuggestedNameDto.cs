
using Core.Dto.Response;

namespace Core.Dto.Request;

public record CreateSuggestedNameDto
{  
    public string Name { get; init; }   
    public string Details { get; init; }
    public string Email { get; init; }
    public List<GeoLocationDto> GeoLocation { get; set; }
    public CreateSuggestedNameDto()
    {
        GeoLocation = new List<GeoLocationDto>();
    }

}
