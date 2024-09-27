using MediatR;
using YorubaOrganization.Core.Cache;
using YorubaOrganization.Core.Events;

namespace Application.EventHandlers
{
    public class ExactEntrySearchedEventHandler : INotificationHandler<ExactEntrySearched>
    {
        public IRecentSearchesCache _recentSearchesCache;

        public ExactEntrySearchedEventHandler(IRecentSearchesCache recentSearchesCache) 
        {
            _recentSearchesCache = recentSearchesCache;
        }

        public async Task Handle(ExactEntrySearched notification, CancellationToken cancellationToken)
        {
            await _recentSearchesCache.Stack(notification.SearchTerm);
        }
    }
}
