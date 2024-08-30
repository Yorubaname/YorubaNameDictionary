using System.Text;
using Tweetinvi;
using Newtonsoft.Json;
namespace Infrastructure.Twitter
{
    public class TwitterClientV2 : ITwitterClientV2
    {
        private readonly ITwitterClient _twitterV1Client;

        public TwitterClientV2(ITwitterClient twitterClient)
        {
            _twitterV1Client = twitterClient;
        }

        public async Task<TweetV2PostResponse> PostTweet(string text)
        {
            var result = await _twitterV1Client.Execute.AdvanceRequestAsync(
                (request) =>
                {
                    var jsonBody = _twitterV1Client.Json.Serialize(new TweetV2PostRequest
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

            return _twitterV1Client.Json.Deserialize<TweetV2PostResponse>(result.Content);
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
}
