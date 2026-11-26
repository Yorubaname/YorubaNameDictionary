using System;
using System.Linq;
using Words.Core.Dto.Response;
using Words.Core.Entities;
using Words.Core.Utilities;
using Xunit;

namespace Test.Words
{
    public class DefinitionGrouperTest
    {
        private static DefinitionDto Def(string content, PartOfSpeech? pos) =>
            new(content, null, [], DateTime.UtcNow, null, pos);

        [Fact]
        public void Group_OrdersGroupsByFirstAppearance()
        {
            var groups = DefinitionGrouper.Group([Def("a", PartOfSpeech.Verb), Def("b", PartOfSpeech.Noun), Def("c", PartOfSpeech.Verb)]);

            Assert.Equal([PartOfSpeech.Verb, PartOfSpeech.Noun], groups.Select(g => g.PartOfSpeech));
        }

        [Fact]
        public void Group_KeepsDefinitionOrderWithinGroup()
        {
            var groups = DefinitionGrouper.Group([Def("a", PartOfSpeech.Verb), Def("b", PartOfSpeech.Noun), Def("c", PartOfSpeech.Verb)]);

            Assert.Equal(["a", "c"], groups[0].Definitions.Select(d => d.Content));
        }

        [Fact]
        public void Group_SingleGroupWhenAllSame() =>
            Assert.Single(DefinitionGrouper.Group([Def("a", PartOfSpeech.Noun), Def("b", PartOfSpeech.Noun)]));

        [Fact]
        public void Group_NullPartOfSpeechFormsOneGroup()
        {
            var group = Assert.Single(DefinitionGrouper.Group([Def("a", null), Def("b", null)]));

            Assert.Null(group.PartOfSpeech);
            Assert.Equal(2, group.Definitions.Count);
        }

        [Fact]
        public void Group_FourDistinctGroups() =>
            Assert.Equal(4, DefinitionGrouper.Group([Def("a", PartOfSpeech.Noun), Def("b", PartOfSpeech.Verb), Def("c", PartOfSpeech.Adjective), Def("d", PartOfSpeech.Adverb)]).Count);

        [Fact]
        public void Group_EmptyInputGivesNoGroups() =>
            Assert.Empty(DefinitionGrouper.Group([]));
    }
}
