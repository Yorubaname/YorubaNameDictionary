using System.Text.Json.Serialization;

namespace Core.Dto.Response.Names
{
    public class NameEntryMiniDto
    {
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Meaning { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SubmittedBy { get; set; }
    }
}
