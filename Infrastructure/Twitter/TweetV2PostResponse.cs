using Newtonsoft.Json;
namespace Infrastructure.Twitter
{
    public record TweetV2PostResponse
    {
        [JsonProperty("data")]
        public TweetV2PostData Data { get; init; }

        public string? Id => Data?.Id;
        public string? Text => Data?.Text;

        public TweetV2PostResponse(TweetV2PostData data)
        {
            Data = data;
        }
    }
}
