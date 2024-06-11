namespace Core.Dto.Response
{
    public record FeedbackDto(string Id, string Name, string Feedback, DateTime SubmittedAt)
    {
    }
}
