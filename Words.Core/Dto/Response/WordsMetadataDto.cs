namespace Words.Core.Dto.Response
{
    public record WordsMetadataDto(
        long TotalWords,
        long TotalNewWords,
        long TotalModifiedWords,
        long TotalPublishedWords);
}
