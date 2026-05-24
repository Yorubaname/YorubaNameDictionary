namespace Api.Configuration
{
    public record WordEtymologyDefinitionsSyncConfig
    {
        public int PageSize { get; init; } = 100;
        public int MaxPagesPerRun { get; init; } = 1000;
        public int WordLogSampleSize { get; init; } = 25;
        public string CheckpointKeyPrefix { get; init; } = "WordEtymologyDefinitionsSync";
    }
}