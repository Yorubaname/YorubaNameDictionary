using Core.Dto.Request;

namespace Core.Dto.Response;

public record SuggestedNameDto
{
    public string? Name { get; set; }
    public string? Details { get; set; }
    public List<GeoLocationDto> GeoLocation { get; set; }
    public string? Email { get; set; }

    public SuggestedNameDto()
    {
        GeoLocation = new List<GeoLocationDto>();
    }
}
