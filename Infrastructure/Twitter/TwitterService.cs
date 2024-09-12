using Application.Services;
using Application.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Infrastructure.Twitter
{
    public class TwitterService(
        ITwitterClientV2 twitterApiClient,
        ILogger<TwitterService> logger,
        IOptions<TwitterConfig> twitterConfig) : ITwitterService
    {
        private readonly ITwitterClientV2 _twitterApiClient = twitterApiClient;
        private readonly ILogger<TwitterService> _logger = logger;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;

        public ValueTask<string> BuildNameTweet(string name, string meaning)
        {
            string link = $"{_twitterConfig.NameUrlPrefix}/{name}";
            return ValueTask.FromResult(_twitterConfig.TweetTemplate
                                .Replace("{name}", name)
                                .Replace("{meaning}", meaning.TrimEnd('.'))
                                .Replace("{link}", link));
        }

        public async Task PostTweet(string text)
        {
            if (!Debugger.IsAttached)
            {
                var tweet = await _twitterApiClient.PostTweet(text);
                if (tweet != null)
                {
                    _logger.LogInformation("Tweet was posted successfully with ID: {tweetId}", tweet.Id);
                }
            }
        }
    }
}
