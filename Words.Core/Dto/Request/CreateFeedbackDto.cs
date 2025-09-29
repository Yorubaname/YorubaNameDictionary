using System.ComponentModel.DataAnnotations;

namespace Words.Core.Dto.Request
{
    public record CreateFeedbackDto([Required] string Word, [Required(ErrorMessage = "Cannot give an empty feedback")] string Feedback);
}
