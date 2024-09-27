using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using YorubaOrganization.Application.Exceptions;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Repositories;

namespace YorubaOrganization.Application.Services
{
    public abstract class DictionaryEntryService<T> where T : DictionaryEntry<T>
    {
        private readonly IDictionaryEntryRepository<T> _entryRepository;
        private readonly IEventPubService _eventPubService;
        private readonly ILogger<DictionaryEntryService<T>> _logger;

        public DictionaryEntryService(
            IDictionaryEntryRepository<T> nameEntryRepository,
            IEventPubService eventPubService,
            ILogger<DictionaryEntryService<T>> logger
            )
        {
            _entryRepository = nameEntryRepository;
            _eventPubService = eventPubService;
            _logger = logger;
        }

        public virtual async Task Create(T entry)
        {
            var name = entry.Title;

            var existingName = await _entryRepository.FindByName(name);
            if (existingName != null)
            {
                existingName.Duplicates.Add(entry);
                await UpdateName(existingName);
                _logger.LogWarning("Someone attempted to create a new name over existing name: {name}.", name);
                return;
            }

            await CreateOrUpdateName(entry);
        }

        public virtual async Task BulkCreate(List<T> entries)
        {
            foreach (var entry in entries)
            {
                await Create(entry);
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
        }

        public virtual async Task<T> CreateOrUpdateName(T entry)
        {
            var updated = await UpdateNameWithUnpublish(entry);

            if (updated == null)
            {
                await _entryRepository.Create(entry);
            }

            return updated ?? entry;
        }

        public virtual async Task<List<T>> SaveNames(List<T> entries)
        {
            var savedNames = new List<T>();
            foreach (var entry in entries)
            {
                savedNames.Add(await CreateOrUpdateName(entry));
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
            return savedNames;
        }

        public virtual async Task<T?> UpdateName(T nameEntry)
        {
            return await _entryRepository.Update(nameEntry.Title, nameEntry);
        }


        public abstract Task PublishEntry(T entry, string username);

        public virtual async Task PublishEntry(T entry, string username, params Expression<Func<T, object>>[] properties)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ClientException("Unexpected. User must be logged in to publish a name!");
            }

            T? updates = entry.Modified;
            string originalName = entry.Title;

            if (updates != null)
            {
                // Copy latest updates to the main object as part of the publish operation.
                entry.Title = updates.Title;
                entry.Pronunciation = updates.Pronunciation;
                entry.IpaNotation = updates.IpaNotation;
                entry.Morphology = updates.Morphology;
                entry.MediaLinks = updates.MediaLinks;
                entry.State = updates.State;
                entry.Etymology = updates.Etymology;
                entry.Videos = updates.Videos;
                entry.GeoLocation = updates.GeoLocation;
                entry.Syllables = updates.Syllables;
                entry.VariantsV2 = updates.VariantsV2;
                entry.MediaLinks = updates.MediaLinks;
                entry.UpdatedBy = username;

                // Copy over type specific properties.
                CopyProperties(updates, entry, properties);

                entry.Modified = null;
            }

            entry.State = State.PUBLISHED;
            await _entryRepository.Update(originalName, entry);
        }

        private void CopyProperties<TSource, TTarget>(TSource source, TTarget target, params Expression<Func<TSource, object>>[] properties)
        {
            foreach (var property in properties)
            {
                var func = property.Compile();
                var value = func(source);

                // Get the property name
                var memberExpression = property.Body as MemberExpression;
                if (memberExpression == null && property.Body is UnaryExpression unary)
                {
                    memberExpression = unary.Operand as MemberExpression;
                }

                if (memberExpression != null)
                {
                    var propertyName = memberExpression.Member.Name;

                    // Set the value on the target object
                    var targetProperty = typeof(TTarget).GetProperty(propertyName);
                    if (targetProperty != null && targetProperty.CanWrite)
                    {
                        targetProperty.SetValue(target, value);
                    }
                }
            }
        }


        public virtual async Task<T?> UpdateNameWithUnpublish(T nameEntry)
        {
            if (nameEntry.State == State.PUBLISHED)
            {
                // Unpublish name if it is currently published since it is awaiting some changes.
                nameEntry.State = State.MODIFIED;
            }

            return await UpdateName(nameEntry);
        }

        /// <summary>
        /// Update an existing T with a new version.
        /// </summary>
        /// <param name="originalEntry"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public virtual async Task<T?> UpdateName(T originalEntry, T newEntry)
        {
            originalEntry.Modified = newEntry;
            return await UpdateNameWithUnpublish(originalEntry);
        }

        public virtual async Task<List<T>> ListNames()
        {
            var result = await _entryRepository.ListAll();
            return [.. result];
        }

        public virtual async Task<MetadataResponse> GetMetadata()
        {
            return await _entryRepository.GetMetadata();
        }

        public virtual async Task<List<T>> FindBy(State state)
        {
            return await _entryRepository.FindByState(state);
        }

        public virtual async Task<IEnumerable<T>> GetAllNames(State? state, string? submittedBy)
        {
            return await _entryRepository.GetAllNames(state, submittedBy);
        }

        public virtual async Task<List<T>> List(State? state, string? submittedBy, int pageNumber, int pageSize)
        {
            return await _entryRepository.List(pageNumber, pageSize, state, submittedBy);
        }

        public virtual async Task<T?> LoadName(string name)
        {
            return await _entryRepository.FindByName(name);
        }

        public virtual async Task<List<T>> LoadNames(string[] names)
        {
            return await _entryRepository.FindByNames(names);
        }

        public virtual async Task Delete(string name)
        {
            await _entryRepository.Delete(name);
            await PublishEntryDeletedEvent(name);
        }

        private async Task PublishEntryDeletedEvent(string title)
        {
            await _eventPubService.PublishEvent(new EntryDeleted(title));
        }

        public virtual async Task<T?> FindByNameAndState(string name, State state) =>
            await _entryRepository.FindByNameAndState(name, state);

        public virtual async Task DeleteNamesBatch(string[] names)
        {
            await _entryRepository.DeleteMany(names);
        }
    }
}