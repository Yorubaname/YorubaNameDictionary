using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EventPubService : IEventPubService
    {
        public async Task PublishEvent(object theEvent)
        {
            // TODO Hafiz: Actually implement this.
            await Task.CompletedTask;
        }
    }
}
