using Core.Events;
using MediatR;

namespace Application.Events
{
    public record NonExistingNameUpdateAttemptedAdapter : NonExistingNameUpdateAttempted, INotification
    {
        public NonExistingNameUpdateAttemptedAdapter(string name) : base(name) { }

        public NonExistingNameUpdateAttemptedAdapter(NonExistingNameUpdateAttempted theEvent) : base(theEvent.Name)
        {
        }
    }
}