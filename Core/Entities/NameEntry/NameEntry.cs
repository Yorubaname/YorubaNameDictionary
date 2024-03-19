using Core.Entities.NameEntry.Collections;
using Core.Enums;

namespace Core.Entities.NameEntry;

public class NameEntry : BaseEntity, IComparable<NameEntry>
{
    public string Name { get; set; }

    public string? Pronunciation { get; set; }
    // Only 3 values exist in the DB for this field at the moment. However, Kola would like to retain it.
    public string? IpaNotation { get; set; }
    public string Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }
    public List<string> Morphology { get; set; }
    public List<string> Media { get; set; }
    public State State { get; set; }

    // Note: Did not migrate TonalMark, Tags, InOtherLanguages intentionally since all values are null in the database (no admin boxes for them)
    public List<Etymology> Etymology { get; set; }
    public List<EmbeddedVideo> Videos { get; set; }
    public List<GeoLocation> GeoLocation { get; set; }

    public List<string> FamousPeople { get; set; }

    public List<string> Syllables { get; set; }

    public List<string> Variants { get; set; }

    /// <summary>
    /// When the current entry is edited, the edited version will be stored here until it is published
    /// </summary>
    public NameEntry? Modified { get; set; }

    /// <summary>
    /// <para>An item is added in here if an attempt is made to create a name which already exists.</para>
    /// <para>Only adding in this feature because it exists in the Java version.</para>
    /// <para>There is no functional application of these objects at the moment.</para>
    /// </summary>
    public List<NameEntry> Duplicates { get; set; }
    public List<Feedback> Feedbacks { get; set; }


    private void InitializeLists()
    {
        Etymology = new List<Etymology>();
        Videos = new List<EmbeddedVideo>();
        Duplicates = new List<NameEntry>();
        Feedbacks = new List<Feedback>();

        Syllables = new List<string>();
        FamousPeople = new List<string>();
        Variants = new List<string>();
        Morphology = new List<string>();
        Media = new List<string>();
    }

    public NameEntry(string name, string meaning)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Meaning = meaning ?? throw new ArgumentNullException(nameof(meaning));

        InitializeLists();
    }
    public NameEntry()
    {
        InitializeLists();
    }

    // TODO: Intentionally removed update method

    public int CompareTo(NameEntry? other)
    {
        return Name.CompareTo(other?.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override bool Equals(object? obj)
    {
        // Standard equality checks
        if (obj is not NameEntry)
        {
            return false;
        }

        NameEntry other = (NameEntry)obj;

        return Name == other.Name;
    }
}