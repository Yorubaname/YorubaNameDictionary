using Application.Domain;
using Application.Events;
using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Tweetinvi;

namespace Infrastructure.Services
{
    public class NamePostingService(
        ConcurrentQueue<PostPublishedNameCommand> nameQueue,
        ITwitterClient twitterApiClient,
        ILogger<NamePostingService> logger,
        NameEntryService nameEntryService,
        IOptions<TwitterConfig> twitterConfig) : BackgroundService
    {
        private const string TweetComposeFailure = "Failed to build tweet for name: {name}. It was not found in the database.";
        private readonly ConcurrentQueue<PostPublishedNameCommand> _nameQueue = nameQueue;
        private readonly ITwitterClient _twitterApiClient = twitterApiClient;
        private readonly ILogger<NamePostingService> _logger = logger;
        private readonly NameEntryService _nameEntryService = nameEntryService;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_nameQueue.TryDequeue(out var indexedName))
                {
                    string? tweetText = await BuildTweet(indexedName.Name);

                    if (tweetText == null)
                    {
                        _logger.LogWarning(TweetComposeFailure, indexedName.Name);
                        continue;
                    }

                    var tweet = await _twitterApiClient.Tweets.PublishTweetAsync(tweetText);
                    if (tweet != null)
                    {
                        _logger.LogInformation("Tweeted: {name} successfully with ID: {tweetId}", indexedName.Name, tweet.IdStr);
                    }
                }

                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task<string?> BuildTweet(string name)
        {
            string link = $"{_twitterConfig.NameUrlPrefix}/{name}";
            var nameEntry = await _nameEntryService.LoadName(name);
            return nameEntry == null ? null :_twitterConfig.TweetTemplate
                                .Replace("{name}", nameEntry.Name)
                                .Replace("{meaning}", nameEntry.Meaning)
                                .Replace("{link}", link);
        }

    }
}
