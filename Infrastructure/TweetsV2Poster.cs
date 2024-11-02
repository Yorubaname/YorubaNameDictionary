using System.Text;
using Tweetinvi.Models;
using Tweetinvi;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class TweetsV2Poster
    {
        // ----------------- Fields ----------------

        private readonly ITwitterClient _client;

        // ----------------- Constructor ----------------

        public TweetsV2Poster(ITwitterClient client)
        {
            _client = client;
        }

        public async Task<TweetV2PostResponse> PostTweet(string text)
        {
            var result = await _client.Execute.AdvanceRequestAsync(
                (ITwitterRequest request) =>
                {
                    var jsonBody = _client.Json.Serialize(new TweetV2PostRequest
                    {
                        Text = text
                    });

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    request.Query.Url = "https://api.twitter.com/2/tweets";
                    request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                    request.Query.HttpContent = content;
                }
            );

            if (!result.Response.IsSuccessStatusCode)
            {
                throw new Exception($"Error when posting tweet:{Environment.NewLine}{result.Content}");
            }

            return _client.Json.Deserialize<TweetV2PostResponse>(result.Content);
        }

        /// <summary>
        /// There are a lot more fields according to:
        /// https://developer.twitter.com/en/docs/twitter-api/tweets/manage-tweets/api-reference/post-tweets
        /// but these are the ones we care about for our use case.
        /// </summary>
        private class TweetV2PostRequest
        {
            [JsonProperty("text")]
            public string Text { get; set; } = string.Empty;
        }
    }

    public record TweetV2PostData
    {
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("text")]
        public string Text { get; init; }

        public  TweetV2PostData(string id, string text)
        {
            Id = id;
            Text = text;
        }
    }

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
