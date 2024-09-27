using MediatR;

namespace YorubaOrganization.Core.Events;

public record class EntryDeleted(string Title) : INotification
{
}