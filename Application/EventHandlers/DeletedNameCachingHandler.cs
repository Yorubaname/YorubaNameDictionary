using Application.Events;
using Core.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers
{
    public class DeletedNameCachingHandler : INotificationHandler<NameDeletedAdapter>
    {
        private readonly IRecentIndexesCache _recentIndexesCache;
        private readonly IRecentSearchesCache _recentSearchesCache;
        private readonly ILogger<DeletedNameCachingHandler> _logger;

        public DeletedNameCachingHandler(
            IRecentIndexesCache recentIndexesCache, 
            IRecentSearchesCache recentSearchesCache,
            ILogger<DeletedNameCachingHandler> logger
            )
        {
            _recentIndexesCache = recentIndexesCache;
            _recentSearchesCache = recentSearchesCache;
            _logger = logger;
        }

        public async Task Handle(NameDeletedAdapter notification, CancellationToken cancellationToken)
        {
            try
            {
                await _recentIndexesCache.Remove(notification.Name);
                await _recentSearchesCache.Remove(notification.Name);              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing deleted name '{name}' from cache.", notification.Name);
            }
        }
    }
}