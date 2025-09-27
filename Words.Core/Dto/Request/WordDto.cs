using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Enums;
using Words.Core.Entities;
using Words.Core.Dto.Response;

namespace Words.Core.Dto.Request
{
    public abstract class WordDto
    {
        // Shared fields (mirroring NameDto)
        public string Word { get; set; } // Analogous to NameDto.Name
        public string? Pronunciation { get; set; }
        public string? IpaNotation { get; set; }
        public State? State { get; set; }
        public List<EtymologyDto> Etymology { get; set; }
        public List<EmbeddedVideoDto> Videos { get; set; }
        public List<CreateGeoLocationDto> GeoLocation { get; set; }
        public HyphenSeparatedString? Syllables { get; set; }
        public CommaSeparatedString? Morphology { get; set; }
        public MediaLinkDto[] MediaLinks { get; set; }
        public VariantDto[] Variants { get; set; }
        public virtual string SubmittedBy { get; set; }

        // Word-specific fields (from WordEntry)
        public PartOfSpeech PartOfSpeech { get; set; }
        public Style? Style { get; set; }
        public GrammaticalFeature? GrammaticalFeature { get; set; }
        public List<DefinitionDto> Definitions { get; set; }

        // Constructors
        public WordDto(string word)
        {
            Word = word ?? throw new ArgumentNullException(nameof(word));
            Etymology = [];
            Videos = [];
            GeoLocation = [];
            Definitions = [];
            MediaLinks = [];
            Variants = [];
        }

        public WordDto()
        {
            Etymology = [];
            Videos = [];
            GeoLocation = [];
            Definitions = [];
            MediaLinks = [];
            Variants = [];
        }
    }
}
