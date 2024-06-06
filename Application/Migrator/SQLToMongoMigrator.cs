using Amazon.Runtime.Internal.Transform;
using Application.Services;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Enums;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Migrator
{
    public class SQLToMongoMigrator
    {
        private readonly IMongoCollection<GeoLocation> _geolocationCollection;
        private readonly IMongoCollection<NameEntry> _nameEntryCollection;
        private readonly IConfiguration _configuration;
        public SQLToMongoMigrator(IMongoDatabase mongoDatabase, IConfiguration configuration)
        {
            _configuration = configuration;
            _geolocationCollection = mongoDatabase.GetCollection<GeoLocation>("GeoLocations");
            _nameEntryCollection = mongoDatabase.GetCollection<NameEntry>("NameEntries");
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
                    _geolocationCollection.DeleteMany(FilterDefinition<GeoLocation>.Empty);
                    _geolocationCollection.InsertMany(documentsToInsert);
                    return $"{documentsToInsert.Count} geolocation records inserted into MongoDB successfully.";
                }
                else
                {
                    return "No data found in MySQL table.";
                }
            }
        }

        public string MigrateNameEntry()
        {
            // Connect to MySQL
            using (MySqlConnection mysqlConn = new MySqlConnection(GetSQLConnectionString()))
            {
                mysqlConn.Open();

                // Select data from MySQL
                string mysqlQuery = "select  created_at, extended_meaning, famous_people,ipa_notation, is_indexed, meaning, media, morphology, pronunciation, submitted_by, syllables, updated_at, variants, name, geo_location_id, state from name_entry";
                MySqlCommand mysqlCommand = new MySqlCommand(mysqlQuery, mysqlConn);
                MySqlDataReader mysqlReader = mysqlCommand.ExecuteReader();


                // Insert data into MongoDB
                List<NameEntry> documentsToInsert = new List<NameEntry>();
                List<String> FamousPeople = new List<String>();
                while (mysqlReader.Read())
                {
                    DateTime.TryParse(mysqlReader["created_at"].ToString(), out DateTime createdDate);
                    DateTime.TryParse(mysqlReader["updated_at"].ToString(), out DateTime updatedDate);

                    documentsToInsert.Add(new NameEntry()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = mysqlReader["name"].ToString(),
                        CreatedAt = createdDate,
                        ExtendedMeaning = mysqlReader["extended_meaning"].ToString(),
                        FamousPeople = FamousPeople, // TODO handle this later
                        IpaNotation = mysqlReader["ipa_notation"].ToString(),
                        Meaning = mysqlReader["meaning"].ToString(),
                        Media = mysqlReader["media"].ToString()?.Split(",").ToList(),
                        Morphology = mysqlReader["morphology"].ToString()?.Split(",").ToList(),
                        Pronunciation = mysqlReader["pronunciation"].ToString(),
                        CreatedBy = mysqlReader["submitted_by"].ToString(),
                        Syllables = mysqlReader["syllables"].ToString()?.Split(",").ToList(),
                        UpdatedAt = updatedDate,
                        Variants = mysqlReader["variants"].ToString()?.Split(",").ToList(),
                        State = GetPublishState(mysqlReader["state"].ToString()),

                    });
                }
                mysqlReader.Close();

                if (documentsToInsert.Count > 0)
                {
                    _nameEntryCollection.DeleteMany(FilterDefinition<NameEntry>.Empty);
                    _nameEntryCollection.InsertMany(documentsToInsert);
                    return $"{documentsToInsert.Count} name records inserted into MongoDB successfully.";
                }
                else
                {
                    return "No data found in MySQL table.";
                }
            }
        }


        private State GetPublishState (string input)
        {
            switch (input)
            {
               case "PUBLISHED":return State.PUBLISHED;
                case "NEW": return State.NEW;
                case "UNPUBLISHED": return State.UNPUBLISHED;
                case "MODIFIED": return State.MODIFIED;
           
                default:
                    return State.PUBLISHED;
                  
            }
        }

        private string GetSQLConnectionString()
        {
            string mysqlConnectionString = _configuration.GetSection("MySQL:Connectionstring").Value;
            return mysqlConnectionString;
        }
    }
}
