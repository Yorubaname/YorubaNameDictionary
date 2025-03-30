using MediatR;
using YorubaOrganization.Core;

namespace Core.Events
{
    public record WordIndexed(string Word, string Definition, string? EnglishTranslation, string Tenant) : IMessage, INotification
    {
    }
}
