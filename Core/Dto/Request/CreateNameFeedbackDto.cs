using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Request
{
    public record CreateNameFeedbackDto([Required] string Name, [Required(ErrorMessage = "Cannot give an empty feedback")] string Feedback);
}
