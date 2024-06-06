using Amazon.Runtime.Internal.Transform;
using Application.Migrator.MigrationDTOs.cs;
using Application.Services;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Enums;
using Dapper;
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
    public class SqlToMongoMigrator
    {
        private readonly IMongoCollection<GeoLocation> _geolocationCollection;
        private readonly IMongoCollection<NameEntry> _nameEntryCollection;
        private readonly IConfiguration _configuration;
        public SqlToMongoMigrator(IMongoDatabase mongoDatabase, IConfiguration configuration)
        {
            _configuration = configuration;
            _geolocationCollection = mongoDatabase.GetCollection<GeoLocation>("GeoLocations");
            _nameEntryCollection = mongoDatabase.GetCollection<NameEntry>("NameEntries");
        }
        public string MigrateGeolocation()
        {
            using var connection = new MySqlConnection(GetSQLConnectionString());
            var geolocation = connection.Query<geolocation>("SELECT place, region FROM geo_location");

            if (geolocation == null)
            {
                return "No data found in MySQL table.";
            }
            List<GeoLocation> documentsToInsert = geolocation.Select(s=> new GeoLocation { Place =s.place, Region =s.region }).ToList();

            _geolocationCollection.DeleteMany(FilterDefinition<GeoLocation>.Empty);
            _geolocationCollection.InsertMany(documentsToInsert);
            return $"{documentsToInsert.Count} geolocation records inserted into MongoDB successfully.";
            
        }
        

        //TODO handle 1 to many mappings i.e geolocation, duplicates, feedback, embedded video
        public string MigrateNameEntry()
        {
            using var connection = new MySqlConnection(GetSQLConnectionString());

            var etymology = connection.Query<etymology>("select name_entry_id, meaning, part from name_entry_etymology");

            var name_entry = connection.Query<nameentry>("select id, created_at, extended_meaning, famous_people,ipa_notation, is_indexed, meaning, media, morphology, pronunciation, submitted_by, syllables, updated_at, variants, name, geo_location_id, state from name_entry");

            if(name_entry == null)  return "No data found in MySQL table.";

            //filter etymology per name
            foreach (var item in name_entry)
            {
                item.etymology = etymology.Where(f => f.name_entry_id == item.id).ToList();
            }
          
            List<NameEntry> documentsToInsert = name_entry.Select(s => new NameEntry()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = s.name,
                //CreatedAt = ((DateTime)s.created_at,
                ExtendedMeaning = s.extended_meaning,
                FamousPeople = s.famous_people.Split(",").ToList(),
                IpaNotation = s.ipa_notation,
                Meaning = s.meaning,
                Media = s.media.Split(",").ToList(),
                Morphology = s.morphology.Split(",").ToList(),
                Pronunciation = s.pronunciation,
                CreatedBy = s.submitted_by,
                Syllables = s.syllables.Split(",").ToList(),
                //UpdatedAt = (DateTime)s.updated_at,
                Variants = s.variants.Split(",").ToList(),
                State = GetPublishState(s.state),
                Etymology = s.etymology.Select(s => new Core.Entities.NameEntry.Collections.Etymology(s.part, s.meaning) { }).ToList(),
            }).ToList();
        
            _nameEntryCollection.DeleteMany(FilterDefinition<NameEntry>.Empty);
            _nameEntryCollection.InsertMany(documentsToInsert);
            return $"{documentsToInsert.Count} name records inserted into MongoDB successfully.";
           
        }


        private State GetPublishState(string input)
        {
            switch (input)
            {
                case "PUBLISHED": return State.PUBLISHED;
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
