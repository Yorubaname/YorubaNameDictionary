namespace Words.Core.Dto.Response
{
    public record WordFeedbackDto(string Id, string Word, string Feedback, DateTime SubmittedAt)
    {
    }
}
