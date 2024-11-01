using MediatR;

namespace YorubaOrganization.Core.Events
{
    public record EntryTitleUpdated(string OriginalTitle, string NewTitle) : INotification
    {
    }
}