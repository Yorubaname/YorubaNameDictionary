using Application.Mappers.Words;
using Core.Dto.Request;
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
    }
}
