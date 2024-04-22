using Core.Events;
using MediatR;

namespace Application.Events;

public record class NameDeletedAdapter : NameDeleted, INotification
{
    public NameDeletedAdapter(string name) : base(name) { }

    public NameDeletedAdapter(NameDeleted theEvent) : base(theEvent.Name)
    {
    }
}