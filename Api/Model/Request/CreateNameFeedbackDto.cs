using System.ComponentModel.DataAnnotations;

namespace Api.Model.Request
{
    public record CreateNameFeedbackDto([Required] string Name, [Required] string FeedbackContent);
}
