using Newtonsoft.Json;
namespace Infrastructure.Twitter
{
    public record TweetV2PostData
    {
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("text")]
        public string Text { get; init; }

        public TweetV2PostData(string id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
