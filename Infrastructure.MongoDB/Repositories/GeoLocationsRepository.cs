using Core.Entities;
using Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.MongoDB.Repositories
{
    public class GeoLocationsRepository : IGeoLocationsRepository
    {
        private readonly IMongoCollection<GeoLocation> _geoLocationsCollection;

        public GeoLocationsRepository(IMongoDatabase database)
        {
            _geoLocationsCollection = database.GetCollection<GeoLocation>("GeoLocations");
            // Check if data exists, if not, initialize with default data
            if (_geoLocationsCollection.CountDocuments(FilterDefinition<GeoLocation>.Empty) == 0)
            {
                InitGeoLocation();
            }
        }
        
        public async Task<GeoLocation> FindByPlace(string place)
        {
            var filter = Builders<GeoLocation>.Filter.Eq("Place", place);
            return await _geoLocationsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<List<GeoLocation>> GetAll()
        {
            return await _geoLocationsCollection.Find(FilterDefinition<GeoLocation>.Empty).ToListAsync();
        }

        private void InitGeoLocation()
        {
            // North-West Yoruba (NWY): Abẹokuta, Ibadan, Ọyọ, Ogun and Lagos (Eko) areas
            // Central Yoruba (CY): Igbomina, Yagba, Ilésà, Ifẹ, Ekiti, Akurẹ, Ẹfọn, and Ijẹbu areas.
            // South-East Yoruba (SEY): Okitipupa, Ilaje, Ondo, Ọwọ, Ikarẹ, Ṣagamu, and parts of Ijẹbu.
            _geoLocationsCollection.InsertMany(new List<GeoLocation>
            {
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
            });
        }
    }
}
