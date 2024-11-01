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
            IDictionaryEntryRepository<T> entryRepository,
            IEventPubService eventPubService,
            ILogger<DictionaryEntryService<T>> logger
            )
        {
            _entryRepository = entryRepository;
            _eventPubService = eventPubService;
            _logger = logger;
        }

        public virtual async Task Create(T entry)
        {
            var title = entry.Title;

            T? existingEntry = await _entryRepository.FindByName(title);
            if (existingEntry != null)
            {
                existingEntry.Duplicates.Add(entry);
                await Update(existingEntry);
                _logger.LogWarning("Someone attempted to create a new entry over existing entry: {title}.", title);
                return;
            }

            await CreateOrUpdate(entry);
        }

        public virtual async Task BulkCreate(List<T> entries)
        {
            foreach (var entry in entries)
            {
                await Create(entry);
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
        }

        public virtual async Task<T> CreateOrUpdate(T entry)
        {
            var updated = await UpdateEntryWithUnpublish(entry);

            if (updated == null)
            {
                await _entryRepository.Create(entry);
            }

            return updated ?? entry;
        }

        public virtual async Task<List<T>> SaveEntries(List<T> entries)
        {
            var savedEntries = new List<T>();
            foreach (var entry in entries)
            {
                savedEntries.Add(await CreateOrUpdate(entry));
                // TODO Later: Ensure that removing batched writes to database here will not cause problems
            }
            return savedEntries;
        }

        public virtual async Task<T?> Update(T entry)
        {
            return await _entryRepository.Update(entry.Title, entry);
        }


        public abstract Task PublishEntry(T entry, string username);

        public virtual async Task PublishEntry(T entry, string username, params Expression<Func<T, object>>[] properties)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ClientException("Unexpected. User must be logged in to publish an entry!");
            }

            T? updates = entry.Modified;
            string originalTitle = entry.Title;

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
            await _entryRepository.Update(originalTitle, entry);
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


        public virtual async Task<T?> UpdateEntryWithUnpublish(T entry)
        {
            if (entry.State == State.PUBLISHED)
            {
                // Unpublish entry if it is currently published since it is awaiting the approval of the new changes.
                entry.State = State.MODIFIED;
            }

            return await Update(entry);
        }

        /// <summary>
        /// Update an existing T with a new version.
        /// </summary>
        /// <param name="originalEntry"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public virtual async Task<T?> Update(T originalEntry, T newEntry)
        {
            originalEntry.Modified = newEntry;
            return await UpdateEntryWithUnpublish(originalEntry);
        }

        public virtual async Task<List<T>> ListAll()
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

        public virtual async Task<IEnumerable<T>> GetAllEntries(State? state, string? submittedBy)
        {
            return await _entryRepository.GetAllNames(state, submittedBy);
        }

        public virtual async Task<List<T>> List(State? state, string? submittedBy, int pageNumber, int pageSize)
        {
            return await _entryRepository.List(pageNumber, pageSize, state, submittedBy);
        }

        public virtual async Task<T?> LoadEntry(string title)
        {
            return await _entryRepository.FindByName(title);
        }

        public virtual async Task<List<T>> LoadEntries(string[] titles)
        {
            return await _entryRepository.FindByNames(titles);
        }

        public virtual async Task Delete(string title)
        {
            await _entryRepository.Delete(title);
            await PublishEntryDeletedEvent(title);
        }

        private async Task PublishEntryDeletedEvent(string title)
        {
            await _eventPubService.PublishEvent(new EntryDeleted(title));
        }

        public virtual async Task<T?> FindByTitleAndState(string title, State state) =>
            await _entryRepository.FindByNameAndState(title, state);

        public virtual async Task DeleteEntriesBatch(string[] titles)
        {
            await _entryRepository.DeleteMany(titles);
        }
    }
}