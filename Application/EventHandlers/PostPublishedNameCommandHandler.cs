using Application.Config;
using Application.Events;
using Application.Services;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Application.EventHandlers
{
    public class PostPublishedNameCommandHandler : INotificationHandler<PostPublishedNameCommand>
    {
        private readonly IMemoryCache _cache;
        private readonly ITwitterService _twitterService;
        private readonly TwitterConfig _twitterConfig;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private const string LastTweetPublishedKey = "LastTweetPublished";

        public PostPublishedNameCommandHandler(
            ITwitterService twitterService,
            IOptions<TwitterConfig> twitterConfig,
            IMemoryCache cache)
        {
            _cache = cache;
            _twitterService = twitterService;
            _twitterConfig = twitterConfig.Value;
        }

        public async Task Handle(PostPublishedNameCommand notification, CancellationToken cancellationToken)
        {
            var theTweet = await _twitterService.BuildNameTweet(notification.Name, notification.Meaning);

            await _semaphore.WaitAsync(cancellationToken);

            var foundLastPublished = _cache.TryGetValue(LastTweetPublishedKey, out DateTimeOffset lastTweetPublished);
            var nextTweetTime = lastTweetPublished.AddSeconds(_twitterConfig.TweetIntervalSeconds);
            
            if(foundLastPublished && nextTweetTime > DateTimeOffset.Now)
            {
                BackgroundJob.Schedule(() => _twitterService.PostTweet(theTweet), nextTweetTime);
            }
            else
            {
                nextTweetTime = DateTimeOffset.Now;
                BackgroundJob.Enqueue(() => _twitterService.PostTweet(theTweet));
            }

            _cache.Set(LastTweetPublishedKey, nextTweetTime);
            _semaphore.Release();
        }
    }
}
