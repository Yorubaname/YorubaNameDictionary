using Core.Entities;
using Core.Events;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using YorubaOrganization.Application.Services;
using YorubaOrganization.Core.Events;

namespace Application.Domain
{
    public class NameEntryService : DictionaryEntryService<NameEntry>
    {
        private readonly INameEntryRepository _nameEntryRepository;
        private readonly IEventPubService _eventPubService;
        private readonly ILogger<NameEntryService> _logger;

        public NameEntryService(
            INameEntryRepository nameEntryRepository,
            IEventPubService eventPubService,
            ILogger<NameEntryService> logger
            ) : base(nameEntryRepository, eventPubService, logger)
        {
            _nameEntryRepository = nameEntryRepository;
            _eventPubService = eventPubService;
            _logger = logger;
        }

        public async Task<List<NameEntry>> BulkUpdateNames(List<NameEntry> nameEntries)
        {
            var updatedNames = new List<NameEntry>();

            // TODO Later: Update all names in one batch
            foreach (var nameEntry in nameEntries)
            {
                var updated = await UpdateEntryWithUnpublish(nameEntry);

                if (updated != null)
                {
                    updatedNames.Add(updated);
                }
                else
                {
                    await _eventPubService.PublishEvent(new NonExistingEntryUpdateAttempted(nameEntry.Title));
                }
            }
            return updatedNames;
        }

        public async override Task PublishEntry(NameEntry nameEntry, string username)
        {
            await base.PublishEntry(nameEntry, username, n => n.Meaning, n => n.ExtendedMeaning, n => n.FamousPeople);
            // TODO Later: Use the outbox pattern to enforce event publishing after the DB update (https://www.youtube.com/watch?v=032SfEBFIJs&t=913s).
            await _eventPubService.PublishEvent(new NameIndexed(nameEntry.Title, nameEntry.Meaning));
        }
    }
}