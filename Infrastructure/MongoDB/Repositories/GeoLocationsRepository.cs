using MongoDB.Bson;
using MongoDB.Driver;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Repositories;
using YorubaOrganization.Core.Tenants;
using YorubaOrganization.Infrastructure;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB.Repositories
{
    public class GeoLocationsRepository : MongoDBRepository<GeoLocation>, IGeoLocationsRepository
    {
        public GeoLocationsRepository(IMongoDatabaseFactory mongoDatabaseFactory, ITenantProvider tenantProvider) :
            base(mongoDatabaseFactory, tenantProvider, "GeoLocations")
        {
            // Check if data exists, if not, initialize with default data
            if (RepoCollection.CountDocuments(FilterDefinition<GeoLocation>.Empty) == 0)
            {
                InitGeoLocation();
            }
        }

        public async Task<GeoLocation> FindByPlace(string place)
        {
            var filter = Builders<GeoLocation>.Filter.Eq(ge => ge.Place, place);
            var options = SetCollationPrimary<FindOptions>(new FindOptions());
            return await RepoCollection.Find(filter, options).SingleOrDefaultAsync();
        }

        public async Task<GeoLocation> FindByPlaceAndRegion(string region, string place)
        {
            var filter = Builders<GeoLocation>.Filter.And(
                Builders<GeoLocation>.Filter.Eq(ge => ge.Region, region),
                Builders<GeoLocation>.Filter.Eq(ge => ge.Place, place)
                );
            var options = SetCollationPrimary<FindOptions>(new FindOptions());
            return await RepoCollection.Find(filter, options).FirstOrDefaultAsync();
        }

        public async Task<List<GeoLocation>> GetAll() => await RepoCollection
                .Find(Builders<GeoLocation>.Filter.Empty)
                .Sort(Builders<GeoLocation>.Sort.Ascending(g => g.Place))
                .ToListAsync();

        public async Task Create(GeoLocation geoLocation)
        {
            geoLocation.Id = ObjectId.GenerateNewId().ToString();
            await RepoCollection.InsertOneAsync(geoLocation);
        }

        public async Task<int> Delete(string id, string place)
        {
            var filterBuilder = Builders<GeoLocation>.Filter;

            var filter = filterBuilder.And(
                filterBuilder.Eq(g => g.Id, id),
                filterBuilder.Eq(g => g.Place, place)
            );

            var options = SetCollationPrimary<DeleteOptions>(new DeleteOptions());
            var deleteResult = await RepoCollection.DeleteOneAsync(filter, options);
            return (int)deleteResult.DeletedCount;
        }

        private void InitGeoLocation()
        {
            // North-West Yoruba (NWY): Abẹokuta, Ibadan, Ọyọ, Ogun and Lagos (Eko) areas
            // Central Yoruba (CY): Igbomina, Yagba, Ilésà, Ifẹ, Ekiti, Akurẹ, Ẹfọn, and Ijẹbu areas.
            // South-East Yoruba (SEY): Okitipupa, Ilaje, Ondo, Ọwọ, Ikarẹ, Ṣagamu, and parts of Ijẹbu.
            RepoCollection.InsertMany(
            [
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "ABEOKUTA",
                    Region = "NWY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IBADAN",
                    Region = "NWY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "OYO",
                    Region = "OYO"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "OSUN",
                    Region = "OYO"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "OGUN",
                    Region = "OGN"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "EKO",
                    Region = "EKO"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IGBOMINA",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "YAGBA",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "ILESHA",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IFE",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "EKITI",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "AKURE",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "EFON",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IJEBU",
                    Region = "CY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "OKITIPUPA",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IJALE",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "ONDO",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "OWO",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "IKARE",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "SAGAMU",
                    Region = "SEY"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "GENERAL/NOT LOCATION SPECIFIC",
                    Region = "GENERAL"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "I DO NOT KNOW",
                    Region = "UNDEFINED"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "FOREIGN: ARABIC",
                    Region = "FOREIGN_ARABIC"
                },
                new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Place = "FOREIGN: GENERAL",
                    Region = "FOREIGN_GENERAL"
                }
            ]);
        }

    }
}
