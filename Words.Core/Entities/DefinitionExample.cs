namespace Words.Core.Entities
{
    public class DefinitionExample
    {
        public required string Content { get; set; }
        public string? EnglishTranslation { get; set; }
        public ExampleType Type { get; set; }
    }
}
