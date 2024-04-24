using Core.Events;
using MediatR;

namespace Application.Events;

public record NameIndexedAdapter : NameIndexed, INotification
{
    public NameIndexedAdapter(NameIndexed theEvent) : base(theEvent.Name)
    {
    }
}