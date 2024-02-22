using Core.Entities;
using Core.Entities.NameEntry;
using Core.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
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
                new ("ABEOKUTA", "NWY"),
                new ("IBADAN", "NWY"),
                new ("OYO", "OYO"),
                new ("OGUN", "OGN"),
                new ("EKO", "EKO"),
                new ("IGBOMINA", "CY"),
                new ("YAGBA", "CY"),
                new ("ILESHA", "CY"),
                new ("IFE", "CY"),
                new ("EKITI", "CY"),
                new ("AKURE", "CY"),
                new ("EFON", "CY"),
                new ("IJEBU", "CY"),
                new ("OKITIPUPA", "SEY"),
                new ("IJALE", "SEY"),
                new ("ONDO", "SEY"),
                new ("OWO", "SEY"),
                new ("IKARE", "SEY"),
                new ("SAGAMU", "SEY"),
                new ("GENERAL/NOT LOCATION SPECIFIC", "GENERAL"),
                new ("I DO NOT KNOW", "UNDEFINED"),
                new ("FOREIGN: ARABIC", "FOREIGN_ARABIC"),
                new ("FOREIGN: GENERAL", "FOREIGN_GENERAL"),
            });
        }
    }
}
