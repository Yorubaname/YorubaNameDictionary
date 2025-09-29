namespace Words.Core.Dto.Response
{
    public record DefinitionDto(string Content, string? EnglishTranslation, List<DefinitionExampleDto> Examples, DateTime SubmittedAt)
    {
    }
}