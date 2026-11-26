using System;
using Application.Validation;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using Xunit;

namespace Test.Words
{
    public class WordValidatorTest
    {
        private static readonly WordValidator Validator = new(
            new GeoLocationValidator(null!),
            new EmbeddedVideoValidator(),
            new EtymologyValidator(),
            new DefinitionValidator());

        private static DefinitionDto Def(PartOfSpeech? pos = null, GrammaticalFeature? gf = null) =>
            new("content", null, [], DateTime.UtcNow, null, pos, gf);

        private static CreateWordDto Word(PartOfSpeech? wordPos, params DefinitionDto[] definitions) =>
            new() { Word = "fayawo", SubmittedBy = "tester", PartOfSpeech = wordPos, Definitions = [.. definitions] };

        [Fact]
        public void Fails_WhenDefinitionHasNoPartOfSpeechAndWordHasNone()
        {
            var result = Validator.Validate(Word(null, Def()));

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Every definition needs a part of speech.");
        }

        [Fact]
        public void Passes_WhenWordLevelPartOfSpeechCoversDefinitions() =>
            Assert.True(Validator.Validate(Word(PartOfSpeech.Noun, Def())).IsValid);

        [Fact]
        public void Passes_WhenEveryDefinitionHasItsOwnAndWordHasNone() =>
            Assert.True(Validator.Validate(Word(null, Def(PartOfSpeech.Noun), Def(PartOfSpeech.Verb, GrammaticalFeature.Transitive))).IsValid);

        [Fact]
        public void Fails_WhenDefinitionPartOfSpeechIsOutOfRange() =>
            Assert.False(Validator.Validate(Word(PartOfSpeech.Noun, Def((PartOfSpeech)999))).IsValid);
    }
}
