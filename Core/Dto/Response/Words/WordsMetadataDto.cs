namespace Core.Dto.Response.Words
{
    public record WordsMetadataDto(
        long TotalWords,
        long TotalNewWords,
        long TotalModifiedWords,
        long TotalPublishedWords);
}
