using System.Text.Json.Serialization;

namespace Core.Dto.Response
{
    public record RecentStats
    {
        [JsonPropertyName("search")]
        public string[] LatestSearches { get; set; }

        [JsonPropertyName("index")]
        public string[] LatestAdditions;

        [JsonPropertyName("popular")]
        public string[] MostPopular;
    }
}
