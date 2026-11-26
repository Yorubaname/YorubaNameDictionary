using Words.Core.Dto.Response;
using Words.Core.Entities;

namespace Words.Core.Utilities
{
    public record PartOfSpeechGroup(PartOfSpeech? PartOfSpeech, List<DefinitionDto> Definitions);

    public static class DefinitionGrouper
    {
        /// <summary>
        /// Groups definitions by part of speech. Groups are ordered by first appearance and
        /// definitions keep their order within a group. Definitions with no part of speech share one group.
        /// </summary>
        public static List<PartOfSpeechGroup> Group(IEnumerable<DefinitionDto> definitions)
        {
            var groups = new List<PartOfSpeechGroup>();

            foreach (var definition in definitions)
            {
                var group = groups.Find(g => g.PartOfSpeech == definition.PartOfSpeech);
                if (group is null)
                {
                    group = new PartOfSpeechGroup(definition.PartOfSpeech, []);
                    groups.Add(group);
                }
                group.Definitions.Add(definition);
            }

            return groups;
        }
    }
}
