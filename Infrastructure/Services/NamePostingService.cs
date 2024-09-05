using Application.Domain;
using Application.Events;
using Infrastructure.Configuration;
using Infrastructure.Twitter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class NamePostingService(
        IEventsQueue eventsQueue,
        ITwitterClientV2 twitterApiClient,
        ILogger<NamePostingService> logger,
        NameEntryService nameEntryService,
        IOptions<TwitterConfig> twitterConfig) : BackgroundService
    {
        private const string TweetComposeFailure = "Failed to build tweet for name: {name}. It was not found in the database.";
        private readonly IEventsQueue _eventsQueue = eventsQueue;
        private readonly ITwitterClientV2 _twitterApiClient = twitterApiClient;
        private readonly ILogger<NamePostingService> _logger = logger;
        private readonly NameEntryService _nameEntryService = nameEntryService;
        private readonly TwitterConfig _twitterConfig = twitterConfig.Value;
        private readonly PeriodicTimer _postingTimer = new (TimeSpan.FromSeconds(twitterConfig.Value.TweetIntervalSeconds));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                PostPublishedNameCommand? indexedName = null;
                try
                {
                    indexedName = await _eventsQueue.Pop<PostPublishedNameCommand>();
                    if (indexedName == null)
                    {
                        continue;
                    }

                    string? tweetText = await BuildTweet(indexedName.Name);

                    if (string.IsNullOrWhiteSpace(tweetText))
                    {
                        _logger.LogWarning(TweetComposeFailure, indexedName.Name);
                        continue;
                    }
                    
                    var tweet = await _twitterApiClient.PostTweet(tweetText);
                    if (tweet != null)
                    {
                        _logger.LogInformation("Tweeted name: {name} successfully with ID: {tweetId}", indexedName.Name, tweet.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to tweet name: `{name}` to Twitter.", indexedName!.Name);
                }
            } while (!stoppingToken.IsCancellationRequested && await _postingTimer.WaitForNextTickAsync(stoppingToken));
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _postingTimer.Dispose();
            await base.StopAsync(stoppingToken);
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
