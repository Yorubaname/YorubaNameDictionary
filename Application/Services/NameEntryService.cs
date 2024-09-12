using Application.Exceptions;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Events;
using Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Domain
{
    public class NameEntryService
    {
        private readonly INameEntryRepository _nameEntryRepository;
        private readonly IEventPubService _eventPubService;
        private readonly ILogger<NameEntryService> _logger;

        public NameEntryService(
            INameEntryRepository nameEntryRepository,
            IEventPubService eventPubService,
            ILogger<NameEntryService> logger
            )
        {
            _nameEntryRepository = nameEntryRepository;
            _eventPubService = eventPubService;
            _logger = logger;
        }

        public async Task Create(NameEntry entry)
        {
            var name = entry.Name;

            var existingName = await _nameEntryRepository.FindByName(name);
            if (existingName != null)
            {
                existingName.Duplicates.Add(entry);
                await UpdateName(existingName);
                _logger.LogWarning("Someone attempted to create a new name over existing name: {name}.", name);
                return;
            }

            await CreateOrUpdateName(entry);
        }

        public async Task BulkCreate(List<NameEntry> entries)
        {
            foreach (var entry in entries)
            {
                await Create(entry);
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
        }

        public async Task<NameEntry> CreateOrUpdateName(NameEntry entry)
        {
            var updated = await UpdateNameWithUnpublish(entry);

            if (updated == null)
            {
                await _nameEntryRepository.Create(entry);
            }

            return updated ?? entry;
        }

        public async Task<List<NameEntry>> SaveNames(List<NameEntry> entries)
        {
            var savedNames = new List<NameEntry>();
            foreach (var entry in entries)
            {
                savedNames.Add(await CreateOrUpdateName(entry));
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
            return savedNames;
        }

        public async Task<NameEntry?> UpdateName(NameEntry nameEntry)
        {
            return await _nameEntryRepository.Update(nameEntry.Name, nameEntry);
        }

        public async Task PublishName(NameEntry nameEntry, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ClientException("Unexpected. User must be logged in to publish a name!");
            }

            NameEntry? updates = nameEntry.Modified;
            string originalName = nameEntry.Name;

            if (updates != null)
            {
                // Copy latest updates to the main object as part of the publish operation.
                nameEntry.Name = updates.Name;
                nameEntry.Pronunciation = updates.Pronunciation;
                nameEntry.IpaNotation = updates.IpaNotation;
                nameEntry.Meaning = updates.Meaning;
                nameEntry.ExtendedMeaning = updates.ExtendedMeaning;
                nameEntry.Morphology = updates.Morphology;
                nameEntry.Media = updates.Media;
                nameEntry.State = updates.State;
                nameEntry.Etymology = updates.Etymology;
                nameEntry.Videos = updates.Videos;
                nameEntry.GeoLocation = updates.GeoLocation;
                nameEntry.FamousPeople = updates.FamousPeople;
                nameEntry.Syllables = updates.Syllables;
                nameEntry.Variants = updates.Variants;
                nameEntry.UpdatedBy = username;

                nameEntry.Modified = null;
            }

            nameEntry.State = State.PUBLISHED;
            await _nameEntryRepository.Update(originalName, nameEntry);

            // TODO Later: Use the outbox pattern to enforce event publishing after the DB update (https://www.youtube.com/watch?v=032SfEBFIJs&t=913s).
            await _eventPubService.PublishEvent(new NameIndexed(nameEntry.Name, nameEntry.Meaning));
        }

        public async Task<NameEntry?> UpdateNameWithUnpublish(NameEntry nameEntry)
        {
            if (nameEntry.State == State.PUBLISHED)
            {
                // Unpublish name if it is currently published since it is awaiting some changes.
                nameEntry.State = State.MODIFIED;
            }

            return await UpdateName(nameEntry);
        }

        /// <summary>
        /// Update an existing NameEntry with a new version.
        /// </summary>
        /// <param name="originalEntry"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public async Task<NameEntry?> UpdateName(NameEntry originalEntry, NameEntry newEntry)
        {
            originalEntry.Modified = newEntry;
            return await UpdateNameWithUnpublish(originalEntry);
        }

        public async Task<List<NameEntry>> BulkUpdateNames(List<NameEntry> nameEntries)
        {
            var updatedNames = new List<NameEntry>();

            // TODO Later: Update all names in one batch
            foreach (var nameEntry in nameEntries)
            {
                var updated = await UpdateNameWithUnpublish(nameEntry);

                if (updated != null)
                {
                    updatedNames.Add(updated);
                }
                else
                {
                    await _eventPubService.PublishEvent(new NonExistingNameUpdateAttempted(nameEntry.Name));
                }
            }
            return updatedNames;
        }

        public async Task<List<NameEntry>> ListNames()
        {
            var result = await _nameEntryRepository.ListAll();
            return result.ToList();
        }

        public async Task<NamesMetadataDto> GetMetadata()
        {
            return await _nameEntryRepository.GetMetadata();
        }

        public async Task<List<NameEntry>> FindBy(State state)
        {
            return await _nameEntryRepository.FindByState(state);
        }

        public async Task<IEnumerable<NameEntry>> GetAllNames(State? state, string? submittedBy)
        {
            return await _nameEntryRepository.GetAllNames(state, submittedBy);
        }

        public async Task<List<NameEntry>> List(State? state, string? submittedBy, int pageNumber, int pageSize)
        {
            return await _nameEntryRepository.List(pageNumber, pageSize, state, submittedBy);
        }

        public async Task<NameEntry?> LoadName(string name)
        {
            return await _nameEntryRepository.FindByName(name);
        }

        public async Task<List<NameEntry>> LoadNames(string[] names)
        {
            return await _nameEntryRepository.FindByNames(names);
        }

        public async Task Delete(string name)
        {
            await _nameEntryRepository.Delete(name);
            await PublishNameDeletedEvent(name);
        }

        private async Task PublishNameDeletedEvent(string name)
        {
            await _eventPubService.PublishEvent(new NameDeleted(name));
        }

        public async Task<NameEntry?> FindByNameAndState(string name, State state) =>
            await _nameEntryRepository.FindByNameAndState(name, state);

        public async Task DeleteNamesBatch(string[] names)
        {
            await _nameEntryRepository.DeleteMany(names);
        }
    }
}