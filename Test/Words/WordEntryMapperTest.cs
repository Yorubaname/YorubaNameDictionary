using System;
using Application.Mappers.Words;
using Words.Core.Dto.Response;
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

        [Fact]
        public void MapToDto_DefinitionWithoutOwnValues_InheritsWordPartOfSpeechAndGrammaticalFeature()
        {
            var entry = new WordEntry
            {
                Title = "word",
                PartOfSpeech = PartOfSpeech.Verb,
                GrammaticalFeature = GrammaticalFeature.Transitive,
                Definitions = [new Definition { Content = "c" }]
            };

            var definition = Assert.Single(entry.MapToDto().Definitions);

            Assert.Equal(PartOfSpeech.Verb, definition.PartOfSpeech);
            Assert.Equal(GrammaticalFeature.Transitive, definition.GrammaticalFeature);
        }

        [Fact]
        public void MapToDto_DefinitionOwnValuesWinOverWord()
        {
            var entry = new WordEntry
            {
                Title = "word",
                PartOfSpeech = PartOfSpeech.Noun,
                Definitions = [new Definition { Content = "c", PartOfSpeech = PartOfSpeech.Verb, GrammaticalFeature = GrammaticalFeature.Intransitive }]
            };

            var definition = Assert.Single(entry.MapToDto().Definitions);

            Assert.Equal(PartOfSpeech.Verb, definition.PartOfSpeech);
            Assert.Equal(GrammaticalFeature.Intransitive, definition.GrammaticalFeature);
        }

        [Fact]
        public void MapToDto_NoValuesAnywhere_LeavesValuesNull()
        {
            var entry = new WordEntry { Title = "word", Definitions = [new Definition { Content = "c" }] };

            var dto = entry.MapToDto();

            Assert.Null(dto.PartOfSpeech);
            Assert.Null(Assert.Single(dto.Definitions).PartOfSpeech);
        }

        [Fact]
        public void MapToEntity_CarriesDefinitionValues_AndAllowsNullWordPartOfSpeech()
        {
            var dto = new CreateWordDto
            {
                Word = "word",
                Definitions = [new DefinitionDto("c", null, [], DateTime.UtcNow, null, PartOfSpeech.Verb, GrammaticalFeature.Transitive)]
            };

            var entry = dto.MapToEntity();

            Assert.Null(entry.PartOfSpeech);
            var definition = Assert.Single(entry.Definitions);
            Assert.Equal(PartOfSpeech.Verb, definition.PartOfSpeech);
            Assert.Equal(GrammaticalFeature.Transitive, definition.GrammaticalFeature);
        }
    }
}
