using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Model.In
{
    /// <summary>
    /// TODO: Add field validations
    /// </summary>
    public class CreateNameDto
    {
        public string Name { get; set; }
        public string? Pronunciation { get; set; }
        public string Meaning { get; set; }
        public string? ExtendedMeaning { get; set; }

        public State? State { get; set; }
        public List<EtymologyDto> Etymology { get; set; }
        public List<EmbeddedVideoDto> Videos { get; set; }
        public List<GeoLocationDto> GeoLocation { get; set; }

        public CommaSeparatedString FamousPeople { get; set; }

        public HyphenSeparatedString Syllables { get; set; }

        public CommaSeparatedString Variants { get; set; }

        public CommaSeparatedString? Morphology { get; set; }

        public CommaSeparatedString? Media { get; set; }


        [Required]
        public string SubmittedBy { get; set; }

        public CreateNameDto(string name, string meaning)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
            Etymology = new List<EtymologyDto>();
            Videos = new List<EmbeddedVideoDto>();
            GeoLocation = new List<GeoLocationDto>();
        }

        public CreateNameDto()
        {
            Etymology = new List<EtymologyDto>();
            Videos = new List<EmbeddedVideoDto>();
            GeoLocation = new List<GeoLocationDto>();
        }
    }

}
