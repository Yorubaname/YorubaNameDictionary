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
        private readonly TweetsV2Poster _twitterApiClient = new (twitterApiClient);
        private readonly ILogger<NamePostingService> _logger = logger;
        private readonly NameEntryService _nameEntryService = nameEntryService;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;
        private const int TweetIntervalMs = 3 * 60 * 1000; // 3 minutes

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_nameQueue.TryDequeue(out var indexedName))
                {
                    await Task.Delay(TweetIntervalMs, stoppingToken);
                    continue;
                }

                string? tweetText = await BuildTweet(indexedName.Name);

                if (string.IsNullOrWhiteSpace(tweetText))
                {
                    _logger.LogWarning(TweetComposeFailure, indexedName.Name);
                    continue;
                }

                try
                {
                    var tweet = await _twitterApiClient.PostTweet(tweetText);
                    if (tweet != null)
                    {
                        _logger.LogInformation("Tweeted name: {name} successfully with ID: {tweetId}", indexedName.Name, tweet.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to tweet name: {name} to Twitter.", indexedName.Name);
                    _nameQueue.Enqueue(indexedName);
                }

                await Task.Delay(TweetIntervalMs, stoppingToken);
            }
        }

        private async Task<string?> BuildTweet(string name)
        {
            string link = $"{_twitterConfig.NameUrlPrefix}/{name}";
            var nameEntry = await _nameEntryService.LoadName(name);
            return nameEntry == null ? null : _twitterConfig.TweetTemplate
                                .Replace("{name}", nameEntry.Name)
                                .Replace("{meaning}", nameEntry.Meaning.TrimEnd('.'))
                                .Replace("{link}", link);
        }

    }
}
