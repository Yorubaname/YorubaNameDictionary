using Words.Core.Entities;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories;

public class WordFeedbackRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider) :
    EntryFeedbackRepository<WordEntry>("Words", databaseFactory, tenantProvider);
