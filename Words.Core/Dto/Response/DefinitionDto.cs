using Words.Core.Entities;

namespace Words.Core.Dto.Response
{
    public record DefinitionDto(string Content, string? EnglishTranslation, List<DefinitionExampleDto> Examples, DateTime SubmittedAt, bool? NeedsReview = null, PartOfSpeech? PartOfSpeech = null, GrammaticalFeature? GrammaticalFeature = null)
    {
    }
}