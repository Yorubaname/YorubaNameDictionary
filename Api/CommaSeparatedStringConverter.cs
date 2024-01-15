using Api.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Api
{
    public class CommaSeparatedStringConverter : JsonConverter<CommaSeparatedString>
    {
        public override CommaSeparatedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new CommaSeparatedString(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, CommaSeparatedString value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
