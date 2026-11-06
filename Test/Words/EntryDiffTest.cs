using System;
using YorubaOrganization.Core.Entities.Partials;
using Words.Core.Entities;
using YorubaOrganization.Core.Utilities;
using YorubaOrganization.Core.Entities;
using YorubaOrganization.Core.Enums;
using Xunit;

namespace Test.Words
{
    public class EntryDiffTest
    {
        private static WordEntry Word() => new()
        {
            Title = "ọba",
            Pronunciation = "o-ba",
            Syllables = ["ọ", "ba"],
            Definitions =
            [
                new Definition { Content = "king", EnglishTranslation = "king" }
            ]
        };

        [Fact]
        public void IdenticalEntries_CountZero()
        {
            Assert.Equal(0, EntryDiff.CountChangedFields(Word(), Word()));
        }

        [Fact]
        public void ChangedScalar_CountsOne()
        {
            var after = Word();
            after.Pronunciation = "ọ-ba";

            Assert.Equal(1, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void AddedAndRemovedValues_BothCount()
        {
            var after = Word();
            after.IpaNotation = "/ɔ.ba/";      // added
            after.Pronunciation = null;        // removed

            Assert.Equal(2, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void EmptyStringAndNull_AreTheSame()
        {
            var before = Word();
            before.IpaNotation = null;
            var after = Word();
            after.IpaNotation = "";

            Assert.Equal(0, EntryDiff.CountChangedFields(before, after));
        }

        [Fact]
        public void ReorderedScalarArray_CountsZero()
        {
            var after = Word();
            after.Syllables = ["ba", "ọ"];

            Assert.Equal(0, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void ScalarArray_CountsEachAddedAndRemovedItem()
        {
            var after = Word();
            after.Syllables = ["ọ", "bá", "a"];

            // "ba" removed, "bá" and "a" added
            Assert.Equal(3, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void NestedDefinitionFields_CountIndividually()
        {
            var after = Word();
            after.Definitions[0].EnglishTranslation = "monarch";
            after.Definitions[0].Examples.Add(new DefinitionExample { Content = "Ọba dé", EnglishTranslation = "The king came" });

            // 1 changed translation + new example with Content, EnglishTranslation and Type
            Assert.Equal(4, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void NewDefinition_CountsItsLeaves()
        {
            var after = Word();
            after.Definitions.Add(new Definition { Content = "ruler", EnglishTranslation = "ruler" });

            Assert.Equal(2, EntryDiff.CountChangedFields(Word(), after));
        }

        [Fact]
        public void NewEntry_CountsAllItsValues()
        {
            // Title, Pronunciation, PartOfSpeech (always set), 2 syllables, definition Content + EnglishTranslation
            Assert.Equal(7, EntryDiff.CountChangedFields(null, Word()));
        }

        [Fact]
        public void BookkeepingFields_AreIgnored()
        {
            var before = Word();
            var after = Word();
            after.Id = "abc";
            after.State = State.PUBLISHED;
            after.UpdatedBy = "someone";
            after.CreatedAt = DateTime.UtcNow.AddDays(-3);
            after.Modified = Word();
            after.Publishers.Add(new AuditStamp("someone"));
            after.Feedbacks.Add(new Feedback { Content = "nice" });
            after.Definitions[0].CreatedAt = DateTime.UtcNow.AddDays(-1);
            after.CreatedFieldsCount = 6;

            Assert.Equal(0, EntryDiff.CountChangedFields(before, after));
        }
    }
}
