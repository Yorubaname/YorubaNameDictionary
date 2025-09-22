using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;

namespace Words.Core.Dto.Response
{
    public record VariantDto(string Word, GeoLocationDto? Geolocation)
    {
    }
}
