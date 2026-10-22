using Application.Mappers.Words;
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
        public void MapToEntity_TrimsTitleAndLowercasesItsFirstLetter(string title, string expectedTitle)
        {
            var entry = new CreateWordDto { Word = title }.MapToEntity();

            Assert.Equal(expectedTitle, entry.Title);
        }
    }
}
