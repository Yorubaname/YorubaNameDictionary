using System.Text.Json.Serialization;
using Words.Core.Dto.Response;

namespace Core.Dto.Response
{
    public class WordEntryMiniDto
    {
        public string Word { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DefinitionDto> Definitions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SubmittedBy { get; set; }
    }
}
