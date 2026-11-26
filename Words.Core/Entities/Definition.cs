using YorubaOrganization.Core.Entities;

namespace Words.Core.Entities
{
    public class Definition : BaseEntity
    {
        public required string Content { get; set; }
        public string? EnglishTranslation { get; set; }
        public bool? NeedsReview { get; set; }
        public PartOfSpeech? PartOfSpeech { get; set; }
        public GrammaticalFeature? GrammaticalFeature { get; set; }
        public List<DefinitionExample> Examples { get; set; } = [];
    }
}
