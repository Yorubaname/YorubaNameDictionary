using Application.Exceptions;
using Core.Dto;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;

namespace Application.Domain
{
    public class NameEntryService
    {
        private const int DefaultPage = 1;
        private const int DefaultListCount = 50;
        private const int MaxListCount = 100; //TODO: Make configurable

        private readonly INameEntryRepository _nameEntryRepository;

        public NameEntryService(INameEntryRepository nameEntryRepository)
        {
            _nameEntryRepository = nameEntryRepository;
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
                    await CreateOrUpdateName(existingName);
                }
                else
                {
                    await CreateOrUpdateName(entry);
                }
            }
            else
            {
                throw new RepositoryAccessException("Given name already exists as a variant entry");
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

        // TODO: Confirm where this method is used. It seems a little unhealthy
        public async Task<NameEntry> CreateOrUpdateName(NameEntry entry)
        {
            var updated = await UpdateName(entry.Name, entry);

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
                // TODO: Ensure that removing batched writes to database here will not cause problems
            }
            return savedNames;
        }

        public async Task<NameEntry?> UpdateName(string originalName, NameEntry newEntry)
        {
            return await _nameEntryRepository.Update(originalName, newEntry);
        }

        public async Task<List<NameEntry>> BulkUpdateNames(List<NameEntry> nameEntries)
        {
            var updatedNames = new List<NameEntry>();
            foreach (var nameEntry in nameEntries)
            {
                var updated = await UpdateName(nameEntry.Name, nameEntry);

                if (updated != null)
                {
                    updatedNames.Add(updated);
                }
                else
                {
                    // TODO: Create a "TriedToUpdateANonExistentName" event
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

        public async Task<List<NameEntry>> FindBy(State? state, int? pageNumber, int? count)
        {
            pageNumber ??= DefaultPage;
            count = Math.Min(count ?? DefaultListCount, MaxListCount);
            return await _nameEntryRepository.List(pageNumber.Value, count.Value, ne => ne.State == state);
        }
    }
}