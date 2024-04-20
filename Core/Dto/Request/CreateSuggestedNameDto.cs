using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Request;

public record CreateSuggestedNameDto
{
    [Required]
    public string Name { get; init; }

    [Required]
    public string Details { get; init; }

    [Required]
    public string Email { get; init; }
    public List<GeoLocationDto> GeoLocation { get; set; }

    public CreateSuggestedNameDto()
    {
        GeoLocation = new List<GeoLocationDto>();
    }

}
