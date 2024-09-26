using YorubaOrganization.Core.Enums;

namespace YorubaOrganization.Core.Entities.Partials
{
    public class MediaLink(string url, string? description = null, MediaType type = MediaType.UNKNOWN)
    {
        public string Url { get; set; } = url;
        public string? Description { get; set; } = description;
        public MediaType Type { get; set; } = type;
    }
}
