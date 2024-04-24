using Core.Events;
using MediatR;

namespace Application.Events;

public record NameEntryNameUpdatedAdapter : NameEntryNameUpdated, INotification
{
    public NameEntryNameUpdatedAdapter(NameEntryNameUpdated theEvent) : base(theEvent.OriginalName, theEvent.NewName)
    {
    }
}