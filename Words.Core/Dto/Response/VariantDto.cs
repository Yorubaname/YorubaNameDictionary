using YorubaOrganization.Core.Dto.Request;

namespace Words.Core.Dto.Response
{
    public record VariantDto(string Word, CreateGeoLocationDto? Geolocation)
    {
    }
}
