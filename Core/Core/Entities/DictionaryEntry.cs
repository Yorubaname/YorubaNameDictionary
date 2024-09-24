using Core.Core.Entities.Parts;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;

namespace Core.Core.Entities
{
    public abstract class DictionaryEntry<T> : BaseEntity, IComparable<T> where T: DictionaryEntry<T>
    {
        /// <summary>
        /// <para>I make this abstract because I want to be able to add attributes to it in child classes.</para>
        /// <para>For example, in NameEntry, this attribute will have value "Name".</para>
        /// </summary>
        public virtual string Title { get; set; }

        // I was going to add the meaning field but then I saw that the main dictionary has definition objects instead of a plain text meaning

        public string? Pronunciation { get; set; }
        public List<string> Syllables { get; set; }

        // Note: Did not migrate TonalMark, Tags, InOtherLanguages intentionally since all values are null in the database (no admin boxes for them)
        public string? IpaNotation { get; set; }
        public List<string> Morphology { get; set; }
        public List<Etymology> Etymology { get; set; }

        // This will satisfy the famous people links requirement for the Yoruba Name Dictionary 
        // This replaces the plain string list in the NameEntry class
        // TODO Now: Create migration plan for Media to MediaLinks
        public List<MediaLink> MediaLinks { get; set; }
        public State State { get; set; }
        public List<EmbeddedVideo> Videos { get; set; }
        public List<GeoLocation> GeoLocation { get; set; }

        // This will replace the plain-text variants list in the NameEntry class
        public List<Variant> VariantsV2 { get; set; }

        public T Modified { get; set; }
        public List<T> Duplicates { get; set; }
        public List<Feedback> Feedbacks { get; set; }

        protected virtual void InitializeLists()
        {
            Etymology = [];
            Videos = [];
            Duplicates = [];
            Feedbacks = [];

            Syllables = [];
            VariantsV2 = [];
            Morphology = [];
            MediaLinks = [];

            GeoLocation = [];
        }

        public DictionaryEntry(string title)
        {
            Title = title;

            InitializeLists();
        }
        public DictionaryEntry()
        {
            InitializeLists();
        }

        public int CompareTo(T? other)
        {
            return Title.CompareTo(other?.Title);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title);
        }

        public override bool Equals(object? obj)
        {
            return obj is T other && other.Title == Title;
        }
    }
}
