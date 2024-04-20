using System.ComponentModel.DataAnnotations;

namespace Api.Model.Request
{
    public record CreateNameFeedbackDto([Required] string Name, [Required(ErrorMessage = "Cannot give an empty feedback")] string FeedbackContent);
}
