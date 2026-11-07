using Application.Mappers.Words;
using Core.Dto.Request;
using Words.Core.Entities;
using Words.Core.Dto.Request;
using Xunit;

namespace Test.Words
{
    public class WordEntryMapperTest
    {
        [Theory]
        [InlineData("  ỌBA  ", "ọBA")]
        [InlineData("  Ẹ̀ṢÙ  ", "ẹ̀ṢÙ")]
        [InlineData("!ỌBA", "!ọBA")]
        [InlineData("  !!!  ", "!!!")]
        public void MapToEntity_NormalizesCreatedWordTitle(string title, string expectedTitle)
        {
            var entry = new CreateWordDto { Word = title }.MapToEntity();

            Assert.Equal(expectedTitle, entry.Title);
        }

        [Theory]
        [InlineData("  ỌBA  ", "ọBA")]
        [InlineData("  Ẹ̀ṢÙ  ", "ẹ̀ṢÙ")]
        public void MapToEntity_NormalizesUpdatedWordTitle(string title, string expectedTitle)
        {
            var entry = new UpdateWordDto { Word = title }.MapToEntity();

            Assert.Equal(expectedTitle, entry.Title);
        }

        [Theory]
        [InlineData("  ỌBA  ", "ọBA")]
        [InlineData("  Ẹ̀ṢÙ  ", "ẹ̀ṢÙ")]
        public void MapToEntity_NormalizesSuggestedWordTitle(string title, string expectedTitle)
        {
            var entry = new CreateWordDto { Word = title }.MapToEntity();

            Assert.Equal(expectedTitle, entry.Title);
        }

        [Fact]
        public void MapToDto_MapsRootUpdatedByAndModifiers()
        {
            var entry = new WordEntry
            {
                Title = "word",
                UpdatedBy = "root-editor",
                Modifiers = [new YorubaOrganization.Core.Entities.AuditStamp("modifier")]
            };

            var dto = entry.MapToDto();

            Assert.Equal("root-editor", dto.UpdatedBy);
            Assert.Equal("modifier", Assert.Single(dto.Modifiers).By);
        }
    }
}
