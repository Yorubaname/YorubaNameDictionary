using Application.Events;
using Application.Exceptions;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Events;
using Core.Repositories;

namespace Application.Domain
{
    public class NameEntryService
    {
        private const int DefaultPage = 1;
        private const int DefaultListCount = 50;
        private const int MaxListCount = 100; //TODO: Make configurable

        private readonly INameEntryRepository _nameEntryRepository;
        private readonly IEventPubService _eventPubService;

        public NameEntryService(
            INameEntryRepository nameEntryRepository,
            IEventPubService eventPubService)
        {
            _nameEntryRepository = nameEntryRepository;
            _eventPubService = eventPubService;
        }

        public async Task Create(NameEntry entry)
        {
            var name = entry.Name;

            if (!await NamePresentAsVariant(name))
            {
                var existingName = await _nameEntryRepository.FindByName(name);
                if (existingName != null)
                {
                    existingName.Duplicates.Add(entry);
                    await UpdateName(existingName);
                }
                else
                {
                    await CreateOrUpdateName(entry);
                }
            }
            else
            {
                throw new DuplicateException("Given name already exists as a variant entry");
            }
        }

        public async Task BulkCreate(List<NameEntry> entries)
        {
            foreach (var entry in entries)
            {
                await Create(entry);
                // TODO: Ensure that removing batched writes to database here will not cause problems
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
                // TODO Hafiz: Ensure that removing batched writes to database here will not cause problems
            }
            return savedNames;
        }

        public async Task<NameEntry?> UpdateName(NameEntry nameEntry)
        {
            return await _nameEntryRepository.Update(nameEntry.Name, nameEntry);
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
            if (originalEntry.Modified != null)
            {
                // This is to ensure that each name has a maximum of one pending update
                throw new UnpublishedNameUpdateException();
            }

            originalEntry.Modified = newEntry;
            return await UpdateNameWithUnpublish(originalEntry);
        }

        public async Task<List<NameEntry>> BulkUpdateNames(List<NameEntry> nameEntries)
        {
            var updatedNames = new List<NameEntry>();
            foreach (var nameEntry in nameEntries)
            {
                // TODO: Cater for possible exception
                var updated = await UpdateNameWithUnpublish(nameEntry);

                if (updated != null)
                {
                    updatedNames.Add(updated);
                }
                else
                {
                    await _eventPubService.PublishEvent(new NonExistingNameUpdateAttempted(nameEntry.Name));
                }
                // TODO: Ensure that removing batched writes to database here will not cause problems
            }
            return updatedNames;
        }

        public async Task<List<NameEntry>> ListNames(int? pageNumber, int? count)
        {
            pageNumber ??= DefaultPage;
            count = Math.Min(count ?? DefaultListCount, MaxListCount);

            return await _nameEntryRepository.List(pageNumber.Value, count.Value);
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

        private async Task<bool> NamePresentAsVariant(string name)
        {
            var variantCount = await _nameEntryRepository.CountWhere(ne => ne.Variants.Contains(name));
            return variantCount > 0;
        }

        public async Task<List<NameEntry>> FindBy(State state, int? pageNumber, int? count)
        {
            pageNumber ??= DefaultPage;
            count = Math.Min(count ?? DefaultListCount, MaxListCount);
            return await _nameEntryRepository.List(pageNumber.Value, count.Value, ne => ne.State == state);
        }

        public async Task<NameEntry?> LoadName(string name)
        {
            return await _nameEntryRepository.FindByName(name);
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
    }
}