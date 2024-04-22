using Core.Events;

namespace Application.Events;

public record NameEntryNameUpdatedAdapter : NameEntryNameUpdated
{
    public NameEntryNameUpdatedAdapter(string originalName, string newName) : base(originalName, newName) { } 
    public NameEntryNameUpdatedAdapter(NameEntryNameUpdated theEvent) : base(theEvent.OriginalName, theEvent.NewName)
    {
    }
}