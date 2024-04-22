using Core.Events;

namespace Application.Events
{
    public record NonExistingNameUpdateAttemptedAdapter : NonExistingNameUpdateAttempted
    {
        public NonExistingNameUpdateAttemptedAdapter(string name) : base(name) { }

        public NonExistingNameUpdateAttemptedAdapter(NonExistingNameUpdateAttempted theEvent) : base(theEvent.Name)
        {
        }
    }
}