using Core.Dto.Request;
using Core.Enums;

namespace Core.Dto.Response
{
    public record NameEntryDto
    {
        public string Name { get; set; }
        public string Meaning { get; set; }
        public string? Pronunciation { get; set; }
        public string? IpaNotation { get; set; }
        public CommaSeparatedString Variants { get; set; }
        public HyphenSeparatedString Syllables { get; set; }
        public string? ExtendedMeaning { get; set; }
        public CommaSeparatedString Morphology { get; set; }

        public List<GeoLocationDto> GeoLocation { get; set; }

        public CommaSeparatedString FamousPeople { get; set; }
        public CommaSeparatedString Media { get; set; }
        public string SubmittedBy { get; set; }

        public List<EtymologyDto> Etymology { get; set; }

        public State State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
