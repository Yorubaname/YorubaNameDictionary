using MediatR;
using YorubaOrganization.Core;

namespace Core.Events
{
    public record NameIndexed(string Name, string Meaning, string Tenant) : IMessage, INotification
    {
    }
}
