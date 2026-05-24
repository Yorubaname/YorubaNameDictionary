using YorubaOrganization.Core.Enums;

namespace Words.Core.Dto.Response
{
    public record WordDefinitionNeedsReviewDto(
        State State,
        string WordTitle,
        DateTime LastDefinitionAddedAt,
        int DefinitionsNeedingReviewCount);
}
