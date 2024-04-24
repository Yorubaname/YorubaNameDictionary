using Core.Events;
using MediatR;

namespace Application.Events;

public record NameDeletedAdapter : NameDeleted, INotification
{
    public NameDeletedAdapter(NameDeleted theEvent) : base(theEvent.Name)
    {
    }
}