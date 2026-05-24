namespace Words.Core.Dto.Response
{
    public record WordDefinitionsNeedingReviewPageDto(
        int Page,
        int Count,
        long TotalItems,
        List<WordDefinitionNeedsReviewDto> Items);
}