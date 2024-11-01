using MediatR;

namespace YorubaOrganization.Core.Events
{
    public record NonExistingEntryUpdateAttempted(string Title) : INotification
    {
    }
}