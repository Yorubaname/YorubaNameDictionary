using Amazon.Runtime.Internal.Transform;
using Application.Migrator.MigrationDTOs.cs;
using Application.Services;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;
using Dapper;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
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
        

        //TODO handle more 1 to many mappings i.e  duplicates, feedback, embedded video
        public string MigrateNameEntry()
        {
            using var connection = new MySqlConnection(GetSQLConnectionString());

            var etymology = connection.Query<etymology>("select name_entry_id, meaning, part from name_entry_etymology");

            var feedbacks = connection.Query<nameentryfeedback>("select id, feedback, name, submitted_at from name_entry_feedback");

            var videos = connection.Query<nameentryvideos>("select name_entry_id, caption, url from name_entry_videos");

            var geolocation = connection.Query<geolocation>("select t.region, t.place from  geo_location t");

            var name_entry_geo_location = connection.Query<name_entry_geo_location>("select name_entry_id, geo_location_place from name_entry_geo_location");

            var geolocationPlace = from a in geolocation
                                   join b in name_entry_geo_location
                                   on a.place equals b.geo_location_place
                                   select new { a.region, a.place, b.name_entry_id };


            var name_entry = connection.Query<nameentry>("select id, created_at, extended_meaning, famous_people,ipa_notation, is_indexed, meaning, media, morphology, pronunciation, submitted_by, syllables, updated_at, variants, name, geo_location_id, state from name_entry");

            if(name_entry == null)  return "No data found in MySQL table.";

        
            foreach (var item in name_entry)
            {
                item.etymology = etymology.Where(f => f.name_entry_id == item.id).ToList()
                    .Select(s => new Etymology(s.part, s.meaning) { }).ToList();

                item.geoLocations = geolocationPlace.Where(d=>d.name_entry_id == item.id).
                    Select(a=> new GeoLocation() { Place =a.place, Region = a.region}).ToList();

                item.feedbacks = feedbacks.Where(d => d.name == item.name).
                    Select(a => new Feedback() { Content = a.feedback }).ToList();
                            
                item.embeddedVideo = videos.Where(d=>d.name_entry_id == item.id).
                    Select(s=> new EmbeddedVideo(s.url, s.caption)).ToList();
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
                Etymology = s.etymology,
                GeoLocation = s.geoLocations,
                Feedbacks = s.feedbacks,
                Videos = s.embeddedVideo

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
                    return State.NEW;

            }
        }

        private string GetSQLConnectionString()
        {
            string mysqlConnectionString = _configuration.GetSection("MySQL:Connectionstring").Value;
            return mysqlConnectionString;
        }
    }
}
