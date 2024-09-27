using System.Text.Json.Serialization;

namespace YorubaOrganization.Core.Dto.Response
{
    public record RecentStats
    {
        [JsonPropertyName("search")]
        public string[] LatestSearches { get; init; }

        [JsonPropertyName("index")]
        public string[] LatestAdditions { get; init; }

        [JsonPropertyName("popular")]
        public string[] MostPopular { get; init; }

        public RecentStats()
        {
            LatestSearches = new string[0];
            LatestAdditions = new string[0];
            MostPopular = new string[0];
        }
    }
}
