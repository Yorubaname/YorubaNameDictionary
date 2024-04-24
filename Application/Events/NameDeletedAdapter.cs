using Core.Events;
using MediatR;

namespace Application.Events;

public record class NameDeletedAdapter : NameDeleted, INotification
{
    public NameDeletedAdapter(NameDeleted theEvent) : base(theEvent.Name)
    {
    }
}