using System.Text.Json;
using System.Text.Json.Serialization;
using Words.Core.Entities;

namespace Words.Core.Dto.Request
{
    /// <summary>
    /// Reads a word style, treating "Other" (sent by the dashboard) as no style.
    /// </summary>
    public class StyleJsonConverter : JsonConverter<Style?>
    {
        private const string OtherStyle = "Other";

        public override bool HandleNull => true;

        public override Style? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected a string for {nameof(Style)}.");
            }

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value) || value.Equals(OtherStyle, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (Enum.TryParse<Style>(value, ignoreCase: true, out var style) && Enum.IsDefined(style))
            {
                return style;
            }

            throw new JsonException($"'{value}' is not a valid {nameof(Style)}.");
        }

        public override void Write(Utf8JsonWriter writer, Style? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.Value.ToString());
        }
    }
}
