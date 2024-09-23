using Core.Entities;
using Core.Entities.NameEntry.Collections;

namespace Application.Migrator.MigrationDTOs.cs
{
    internal class nameentry
    {
        public int id { get; set; }
        public byte[] created_at { get; set; }
        public byte[] updated_at { get; set; }
        public string extended_meaning { get; set; }
        public string famous_people { get; set; }
        public string meaning { get; set; }
        public string media { get; set; }
        public string morphology { get; set; }
        public string pronunciation { get; set; }
        public string submitted_by { get; set; }
        public string syllables { get; set; }
        public string variants { get; set; }
        public string name { get; set; }
        public string geo_location_id { get; set; }
        public string state { get; set; }
        public string ipa_notation { get; set; }
        public List<Etymology> etymology { get; set; }
        public List<GeoLocation> geoLocations { get; set; }
        public List<Feedback> feedbacks { get; set; }
        public List<EmbeddedVideo> embeddedVideo { get; set; }
    }
}
