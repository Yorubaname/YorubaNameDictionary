using Core.Events;
using MediatR;

namespace Application.Events
{
    public record ExactNameSearchedAdapter : ExactNameSearched, INotification
    {
        public ExactNameSearchedAdapter(ExactNameSearched theEvent) : base(theEvent.SearchTerm)
        {
        }
    }
}