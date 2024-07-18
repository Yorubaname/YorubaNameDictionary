using Application.Migrator.MigrationDTOs.cs;
using Core.Dto;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MySqlConnector;

namespace Application.Migrator
{
    [Obsolete("Used for initial migration of sql to mongo.. Do not use!")]
    public class SqlToMongoMigrator
    {
        private const string MigratorProcessName = "SQLTOMongoMigrator";
        private readonly IMongoCollection<GeoLocation> _geolocationCollection;
        private readonly IMongoCollection<NameEntry> _nameEntryCollection;
        private readonly IMongoCollection<SuggestedName> _suggestedNameEntryCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IConfiguration _configuration;
        public SqlToMongoMigrator(IMongoDatabase mongoDatabase, IConfiguration configuration)
        {
            _configuration = configuration;
            _geolocationCollection = mongoDatabase.GetCollection<GeoLocation>("GeoLocations");
            _nameEntryCollection = mongoDatabase.GetCollection<NameEntry>("NameEntries");
            _suggestedNameEntryCollection = mongoDatabase.GetCollection<SuggestedName>("SuggestedNames");
            _userCollection = mongoDatabase.GetCollection<User>("Users");
        }
        public string MigrateGeolocation()
        {
            throw new NotImplementedException("Migrate Geolocation Failed.");
            using var connection = new MySqlConnection(GetSQLConnectionString());
            var geolocation = connection.Query<geolocation>("SELECT place, region FROM geo_location");

            if (geolocation == null)
            {
                return "No data found in MySQL table.";
            }
            List<GeoLocation> documentsToInsert = geolocation.Select(s => new GeoLocation
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Place = s.place,
                Region = s.region,
                CreatedBy = MigratorProcessName
            }).ToList();

            _geolocationCollection.DeleteMany(FilterDefinition<GeoLocation>.Empty);
            _geolocationCollection.InsertMany(documentsToInsert);
            return $"{documentsToInsert.Count} geolocation records inserted into MongoDB successfully.";

        }

        public string MigrateNameEntry()
        {
            throw new NotImplementedException("Migrate NameEntries Failed.");
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


            var name_entry = connection.Query<nameentry>("select id, created_at, extended_meaning, famous_people,ipa_notation, is_indexed, " +
                "meaning, media, morphology, pronunciation, submitted_by, syllables, " +
                "updated_at, variants, name, geo_location_id, state " +
                "from name_entry");

            if (name_entry == null) return "No data found in MySQL table.";


            foreach (var item in name_entry)
            {
                item.etymology = etymology.Where(f => f.name_entry_id == item.id).ToList()
                    .Select(s => new Etymology(s.part, s.meaning) { }).ToList();

                item.geoLocations = geolocationPlace.Where(d => d.name_entry_id == item.id).
                    Select(a => new GeoLocation() { Place = a.place, Region = a.region }).ToList();

                item.feedbacks = feedbacks.Where(d => d.name == item.name).
                    Select(a => new Feedback() { Id = $"{a.id}", Content = a.feedback }).ToList();

                item.embeddedVideo = videos.Where(d => d.name_entry_id == item.id).
                    Select(s => new EmbeddedVideo(s.url, s.caption)).ToList();
            }

            List<NameEntry> documentsToInsert = name_entry.Select(s => new NameEntry()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = s.name,
                ExtendedMeaning = s.extended_meaning,
                FamousPeople = new CommaSeparatedString(s.famous_people),
                IpaNotation = s.ipa_notation,
                Meaning = s.meaning,
                Media = new CommaSeparatedString(s.media),
                Morphology = new CommaSeparatedString(s.morphology),
                Pronunciation = s.pronunciation,
                CreatedBy = s.submitted_by,
                Syllables = new HyphenSeparatedString(s.syllables),
                Variants = new CommaSeparatedString(s.variants),
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

        public string MigrateSuggestedNames()
        {
            throw new NotImplementedException("Migrate SuggestedNames Failed.");
            using var connection = new MySqlConnection(GetSQLConnectionString());

            var suggested_name = connection.Query<suggested_name>(
                "select Id, details, email, name, geo.geo_location_place place, gg.region " +
                "from suggested_name n " +
                "left join suggested_name_geo_location geo on n.id = geo.suggested_name_id " +
                "left join geo_location gg on geo.geo_location_place = gg.place");

            if (suggested_name == null) return "No data found in MySQL table.";

            List<SuggestedName> suggestedNamesToInsert = suggested_name.Select(s => new SuggestedName()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = s.name,
                Details = s.details,
                Email = s.email,
                GeoLocation = s.place == null ? new List<GeoLocation>() : new List<GeoLocation> { new GeoLocation(s.place, s.region) },
                CreatedBy = MigratorProcessName
            }).ToList();

            _suggestedNameEntryCollection.DeleteMany(FilterDefinition<SuggestedName>.Empty);
            _suggestedNameEntryCollection.InsertMany(suggestedNamesToInsert);
            return $"{suggestedNamesToInsert.Count} name records inserted into MongoDB successfully.";

        }


        public string MigrateUsers()
        {
            throw new NotImplementedException("Migrate Users Failed.");
            using var connection = new MySqlConnection(GetSQLConnectionString());

            var user = connection.Query<users>("select email, password, roles, username from api_user");

            if (user == null) return "No data found for users";

            List<User> usersToMigrate = user.Select(s => new User()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Email = s.email,
                Username = s.username,
                Password = s.password,
                Roles = s.roles.Split(",").Select(d => d.Trim().Substring(5, d.Trim().Length - 5)).ToList(),
                CreatedBy = MigratorProcessName
            }).ToList();

            _userCollection.DeleteMany(FilterDefinition<User>.Empty);
            _userCollection.InsertMany(usersToMigrate);

            return $"{usersToMigrate.Count} users records inserted into MongoDB successfully.";

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
