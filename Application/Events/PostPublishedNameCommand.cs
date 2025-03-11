using MediatR;
using YorubaOrganization.Core;

namespace Application.Events
{
    public record PostPublishedNameCommand(string Name, string Meaning, string Tenant) : IMessage, INotification
    {
    }
}
