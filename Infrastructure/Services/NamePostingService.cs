using Application.Domain;
using Application.Events;
using Infrastructure.Configuration;
using Infrastructure.Twitter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Infrastructure.Services
{
    public class NamePostingService(
        ConcurrentQueue<PostPublishedNameCommand> nameQueue,
        ITwitterClientV2 twitterApiClient,
        ILogger<NamePostingService> logger,
        NameEntryService nameEntryService,
        IOptions<TwitterConfig> twitterConfig) : BackgroundService
    {
        private const string TweetComposeFailure = "Failed to build tweet for name: {name}. It was not found in the database.";
        private readonly ConcurrentQueue<PostPublishedNameCommand> _nameQueue = nameQueue;
        private readonly ITwitterClientV2 _twitterApiClient = twitterApiClient;
        private readonly ILogger<NamePostingService> _logger = logger;
        private readonly NameEntryService _nameEntryService = nameEntryService;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tweetIntervalMs = _twitterConfig.TweetIntervalSeconds * 1000;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_nameQueue.TryDequeue(out var indexedName))
                {
                    await Task.Delay(tweetIntervalMs, stoppingToken);
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

                await Task.Delay(tweetIntervalMs, stoppingToken);
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
