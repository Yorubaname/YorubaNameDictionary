using YorubaOrganization.Core.Entities;

namespace Words.Core.Entities
{
    public class WordEntry : DictionaryEntry<WordEntry>
    {
        public PartOfSpeech PartOfSpeech { get; set; }
        public Style? Style { get; set; }
        public GrammaticalFeature? GrammaticalFeature { get; set; }

        public List<Definition> Definitions { get; set; }

        protected override void InitializeLists()
        {
            base.InitializeLists();
            Definitions = [];
        }
    }
}
