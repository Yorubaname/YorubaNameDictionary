using Application.Services;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Migrator
{
    public class SQLToMongoMigrator
    {
        private readonly IMongoCollection<GeoLocation> _geoLocationsCollection;
        private readonly IConfiguration _configuration;
        public SQLToMongoMigrator(IMongoDatabase mongoDatabase, IConfiguration configuration)
        {
            _configuration = configuration;
            _geoLocationsCollection = mongoDatabase.GetCollection<GeoLocation>("GeoLocations");
        }
        public string MigrateGeolocation()
        {
            // Connect to MySQL
            using (MySqlConnection mysqlConn = new MySqlConnection(GetSQLConnectionString()))
            {
                mysqlConn.Open();

                // Select data from MySQL
                string mysqlQuery = "SELECT place, region FROM geo_location";
                MySqlCommand mysqlCommand = new MySqlCommand(mysqlQuery, mysqlConn);
                MySqlDataReader mysqlReader = mysqlCommand.ExecuteReader();


                // Insert data into MongoDB
                List<GeoLocation> documentsToInsert = new List<GeoLocation>();
                while (mysqlReader.Read())
                {
                    documentsToInsert.Add(new GeoLocation()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Place = mysqlReader["place"].ToString(),
                        Region = mysqlReader["region"].ToString()
                    });
                }
                mysqlReader.Close();

                if (documentsToInsert.Count > 0)
                {
                    _geoLocationsCollection.DeleteMany(FilterDefinition<GeoLocation>.Empty);
                    _geoLocationsCollection.InsertMany(documentsToInsert);
                    return $"{documentsToInsert.Count} geolocation records inserted into MongoDB successfully.";
                }
                else
                {
                    return "No data found in MySQL table.";
                }
            }
        }



        private string GetSQLConnectionString()
        {
            string mysqlConnectionString = _configuration.GetSection("MySQL:Connectionstring").Value;
            return mysqlConnectionString;
        }
    }
}
