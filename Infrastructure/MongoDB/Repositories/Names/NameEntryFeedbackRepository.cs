using Core.Entities;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories.Names;

public class NameEntryFeedbackRepository(IMongoDatabaseFactory databaseFactory, ITenantProvider tenantProvider) :
    EntryFeedbackRepository<NameEntry>("NameEntries", databaseFactory, tenantProvider);
