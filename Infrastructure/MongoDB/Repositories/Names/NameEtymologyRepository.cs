using Core.Entities;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories.Names;

public class NameEtymologyRepository(IMongoDatabaseFactory mongoDatabaseFactory, ITenantProvider tenantProvider) :
    EtymologyRepository<NameEntry>("NameEntries", mongoDatabaseFactory, tenantProvider);