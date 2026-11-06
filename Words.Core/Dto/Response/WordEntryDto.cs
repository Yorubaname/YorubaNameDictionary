using YorubaOrganization.Core.Dto;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;

namespace Words.Core.Dto.Response
{
    public record WordEntryDto
    {
        public string Key { get; set; }
        public string Id { get; set; }
        public string Word { get; set; }
        public string? Pronunciation { get; set; }
        public string PartOfSpeech { get; set; }
        public string? Style { get; set; }
        public string? GrammaticalFeature { get; set; }
        public string? IpaNotation { get; set; }
        public List<VariantDto> Variants { get; set; }
        public HyphenSeparatedString Syllables { get; set; }
        public CommaSeparatedString Morphology { get; set; }
        public List<GeoLocationDto> GeoLocation { get; set; }
        public string? SubmittedBy { get; set; }
        public List<EtymologyDto> Etymology { get; set; }
        public List<MediaLinkDto> MediaLinks { get; set; }
        public List<DefinitionDto> Definitions { get; set; }
        public State State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CreatedFieldsCount { get; set; }
        public List<AuditStamp> Publishers { get; set; } = [];
        public List<AuditStamp> Modifiers { get; set; } = [];
        public List<AuditStamp> Unpublishers { get; set; } = [];
    }
}
