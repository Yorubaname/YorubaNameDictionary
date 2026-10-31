using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Dto.Request;
using Words.Core.Dto.Request;
using Words.Core.Entities;
using Xunit;

namespace Test.Words
{
    public class StyleJsonConverterTest
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        [Theory]
        [InlineData("\"Colloquial\"", Style.Colloquial)]
        [InlineData("\"Formal\"", Style.Formal)]
        [InlineData("\"formal\"", Style.Formal)]
        [InlineData("\"Other\"", null)]
        [InlineData("\"\"", null)]
        [InlineData("null", null)]
        public void Deserialize_MapsDashboardStyles(string styleJson, Style? expected)
        {
            var dto = JsonSerializer.Deserialize<UpdateWordDto>($"{{\"word\":\"ajodun\",\"style\":{styleJson}}}", Options);

            Assert.Equal(expected, dto!.Style);
        }

        [Fact]
        public void Deserialize_RejectsUnknownStyle()
        {
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<UpdateWordDto>("{\"word\":\"ajodun\",\"style\":\"Slang\"}", Options));
        }

        [Theory]
        [InlineData("Intransitive", GrammaticalFeature.Intransitive)]
        [InlineData("Transitive", GrammaticalFeature.Transitive)]
        public void Deserialize_MapsGrammaticalFeatures(string feature, GrammaticalFeature expected)
        {
            var dto = JsonSerializer.Deserialize<UpdateWordDto>($"{{\"word\":\"ajodun\",\"grammaticalFeature\":\"{feature}\"}}", Options);

            Assert.Equal(expected, dto!.GrammaticalFeature);
        }
    }
}
