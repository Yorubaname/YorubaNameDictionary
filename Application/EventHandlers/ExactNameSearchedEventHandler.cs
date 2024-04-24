using Application.Events;
using Core.Cache;
using MediatR;

namespace Application.EventHandlers
{
    public class ExactNameSearchedEventHandler : INotificationHandler<ExactNameSearchedAdapter>
    {
        public IRecentSearchesCache _recentSearchesCache;

        public ExactNameSearchedEventHandler(IRecentSearchesCache recentSearchesCache) 
        {
            _recentSearchesCache = recentSearchesCache;
        }

        public async Task Handle(ExactNameSearchedAdapter notification, CancellationToken cancellationToken)
        {
            await _recentSearchesCache.Stack(notification.SearchTerm);
        }
    }
}
