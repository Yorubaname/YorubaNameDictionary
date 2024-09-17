using Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Hangfire;
using Infrastructure.Configuration;
using Core.Cache;

namespace Infrastructure.Twitter
{
    public class TwitterService(
        ITwitterClientV2 twitterApiClient,
        ILogger<TwitterService> logger,
        IOptions<TwitterConfig> twitterConfig,
        ISimpleCache cache,
        IBackgroundJobClientV2 backgroundJobClient) : ITwitterService
    {
        private readonly ITwitterClientV2 _twitterApiClient = twitterApiClient;
        private readonly ILogger<TwitterService> _logger = logger;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;
        private readonly ISimpleCache _simpleCache = cache;
        private readonly IBackgroundJobClientV2 _backgroundJobClient = backgroundJobClient;
        private static readonly SemaphoreSlim _semaphore;
        private const string LastTweetPublishedKey = "LastTweetPublished";

        static TwitterService()
        {
            _semaphore = new(1, 1);
        }

        private string BuildNameTweet(string name, string meaning)
        {
            string link = $"{_twitterConfig.NameUrlPrefix}/{name}";
            return _twitterConfig.TweetTemplate
                                .Replace("{name}", name)
                                .Replace("{meaning}", meaning.TrimEnd('.'))
                                .Replace("{link}", link);
        }

        public async Task PostNewNameAsync(string name, string meaning, CancellationToken cancellationToken)
        {
            var theTweet = BuildNameTweet(name, meaning);
            await PostTweetAsync(theTweet, cancellationToken);
        }

        private async Task PostTweetAsync(string tweetText, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken); // We want to be scheduling only one tweet at a time.
            try
            {
                var lastTweetPublished = await _simpleCache.GetAsync<DateTimeOffset>(LastTweetPublishedKey);
                var nextTweetTime = lastTweetPublished.AddSeconds(_twitterConfig.TweetIntervalSeconds);

                if (lastTweetPublished != default && nextTweetTime > DateTimeOffset.Now)
                {
                    _backgroundJobClient.Schedule(() => SendTweetAsync(tweetText), nextTweetTime);
                }
                else
                {
                    nextTweetTime = DateTimeOffset.Now;
                    _backgroundJobClient.Enqueue(() => SendTweetAsync(tweetText));
                }

                await _simpleCache.SetAsync(LastTweetPublishedKey, nextTweetTime);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task SendTweetAsync(string tweetText)
        {
            if (!Debugger.IsAttached) // To prevent tweets from getting posted while testing. Could be better, but...
            {
                var tweet = await _twitterApiClient.PostTweet(tweetText);
                if (tweet != null)
                {
                    _logger.LogInformation("Tweet was posted successfully with ID: {tweetId}", tweet.Id);
                }
            }
        }
    }
}
