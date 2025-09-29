using Words.Core.Entities;

namespace Words.Core.Dto.Response
{
    public record DefinitionExampleDto(string Content, string? EnglishTranslation, ExampleType Type)
    {
    }
}