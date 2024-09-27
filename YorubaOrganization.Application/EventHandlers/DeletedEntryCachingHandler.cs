using MediatR;
using Microsoft.Extensions.Logging;
using YorubaOrganization.Core.Cache;
using YorubaOrganization.Core.Events;

namespace Application.EventHandlers
{
    public class DeletedEntryCachingHandler : INotificationHandler<EntryDeleted>
    {
        private readonly IRecentIndexesCache _recentIndexesCache;
        private readonly IRecentSearchesCache _recentSearchesCache;
        private readonly ILogger<DeletedEntryCachingHandler> _logger;

        public DeletedEntryCachingHandler(
            IRecentIndexesCache recentIndexesCache, 
            IRecentSearchesCache recentSearchesCache,
            ILogger<DeletedEntryCachingHandler> logger
            )
        {
            _recentIndexesCache = recentIndexesCache;
            _recentSearchesCache = recentSearchesCache;
            _logger = logger;
        }

        public async Task Handle(EntryDeleted notification, CancellationToken cancellationToken)
        {
            try
            {
                await _recentIndexesCache.Remove(notification.Title);
                await _recentSearchesCache.Remove(notification.Title);              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing deleted entry '{title}' from cache.", notification.Title);
            }
        }
    }
}