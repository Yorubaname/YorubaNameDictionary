using Core.Entities.NameEntry.Collections;
using Core.Entities;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Model.In
{
    /// <summary>
    /// Add field validations
    /// </summary>
    public class CreateNameDto
    {
        public string Name { get; set; }
        public string? Pronunciation { get; set; }
        public string Meaning { get; set; }
        public string? ExtendedMeaning { get; set; }

        public CommaSeparatedString? Morphology { get; set; }

        public CommaSeparatedString? Media { get; set; }

        public State? State { get; set; }
        public List<Etymology> Etymology { get; set; }
        public List<EmbeddedVideo> Videos { get; set; }
        public List<GeoLocation> GeoLocation { get; set; }

        public CommaSeparatedString FamousPeople { get; set; }

        public CommaSeparatedString Syllables { get; set; }

        public CommaSeparatedString Variants { get; set; }

        [Required]
        public string SubmittedBy { get; set; }

        public CreateNameDto(string name, string meaning)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
            Etymology = new List<Etymology>();
            Videos = new List<EmbeddedVideo>();
            GeoLocation = new List<GeoLocation>();
        }

        public CreateNameDto()
        {
            Etymology = new List<Etymology>();
            Videos = new List<EmbeddedVideo>();
            GeoLocation = new List<GeoLocation>();
        }
    }

}
