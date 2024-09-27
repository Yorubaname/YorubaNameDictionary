using MediatR;

namespace YorubaOrganization.Core.Events
{
    public record ExactEntrySearched(string SearchTerm) : INotification
    {

    }
}