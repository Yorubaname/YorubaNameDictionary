using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Entities.Partials;
using YorubaOrganization.Core.Enums;

namespace Core.Dto.Request
{
    public abstract class NameDto
    {
        public string Name { get; set; }
        public string? Pronunciation { get; set; }
        public string Meaning { get; set; }
        public string? ExtendedMeaning { get; set; }

        public State? State { get; set; }
        public List<EtymologyDto> Etymology { get; set; }
        public List<EmbeddedVideoDto> Videos { get; set; }
        public List<CreateGeoLocationDto> GeoLocation { get; set; }

        public CommaSeparatedString? FamousPeople { get; set; }

        public HyphenSeparatedString? Syllables { get; set; }

        public CommaSeparatedString? Variants { get; set; }

        public List<Variant> VariantsV2 { 
            get
            {
                return Variants == null ? [] : ((List<string>)Variants).Select(s => new Variant(s)).ToList();
            }
        }

        public List<MediaLink> MediaLinks
        {
            get
            {
                return Media == null ? [] : ((List<string>)Media).Select(s => new MediaLink(s)).ToList();
            }
        }

        public CommaSeparatedString? Morphology { get; set; }

        public CommaSeparatedString? Media { get; set; }

        public virtual string SubmittedBy { get; set; }

        public NameDto(string name, string meaning)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));
            Etymology = new List<EtymologyDto>();
            Videos = new List<EmbeddedVideoDto>();
            GeoLocation = new List<CreateGeoLocationDto>();
        }

        public NameDto()
        {
            Etymology = new List<EtymologyDto>();
            Videos = new List<EmbeddedVideoDto>();
            GeoLocation = new List<CreateGeoLocationDto>();
        }
    }
}
