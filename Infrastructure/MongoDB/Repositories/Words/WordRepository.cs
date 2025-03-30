using Core.Entities;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Core.Utilities;
using YorubaOrganization.Infrastructure.Repositories;
using YorubaOrganization.Infrastructure;
using Words.Core.Entities;
using Core.Repositories.Words;

namespace Infrastructure.MongoDB.Repositories.Words
{
    public class WordRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider, IEventPubService eventPubService) : DictionaryEntryRepository<WordEntry>(CollectionName, databaseFactory, tenantProvider, eventPubService), IWordEntryRepository
    {
        private const string CollectionName = "Words";

        // TODO YDict: Implement the custom FindBy methods (which are commented out in the interface) here (based on definitions).

        protected override UpdateDefinition<WordEntry> GenerateCustomUpdateStatement(WordEntry newEntry) => Builders<WordEntry>.Update
                        .Set(ne => ne.PartOfSpeech, newEntry.PartOfSpeech)
                        .Set(ne => ne.Style, newEntry.Style)
                        .Set(ne => ne.GrammaticalFeature, newEntry.GrammaticalFeature)
                        .Set(ne => ne.Definitions, newEntry.Definitions);
    }
}
