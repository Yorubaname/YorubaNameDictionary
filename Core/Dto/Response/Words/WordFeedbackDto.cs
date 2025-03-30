using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Response.Words
{
    public record WordFeedbackDto(string Id, string Word, string Feedback, DateTime SubmittedAt)
    {
    }
}
