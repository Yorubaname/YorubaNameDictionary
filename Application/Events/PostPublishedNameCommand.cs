using MediatR;

namespace Application.Events
{
    public record PostPublishedNameCommand(string Name) : INotification
    {
    }
}
