using YorubaOrganization.Core.Enums;

namespace Words.Core.Dto.Response
{
    public record MediaLinkDto(string Link, string? Caption, MediaType Type)
    {
    }
}