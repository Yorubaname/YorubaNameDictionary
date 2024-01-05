using Core.Enums;

namespace Core.Entities;

public class NameEntry : Entity , IComparable<NameEntry>
{
    public string? Name { get; set; }

    public string? Pronunciation { get; set; }
    // TODO: Clarify need to retain this field; Only 5 records exist in database at the moment
    public string? IpaNotation { get; set; }
    public string Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }
    public string? Morphology { get; set; }
    public string? Media { get; set; }
    public State? State { get; set; }

    // Note: Did not migrate TonalMark, Tags, InOtherLanguages intentionally since all values are null in the database (no admin boxes for them)
    public List<Etymology> Etymology { get; set; }
    public List<EmbeddedVideo> Videos { get; set; }
    public List<GeoLocation> GeoLocation { get; set; }
    
    // TODO: Previously comma separated
    public List<string> FamousPeople { get; set; }

    // TODO: Previously hyphen separated
    public List<string> Syllables { get; set; }
    
    // TODO: Previously comma separated 
    public List<string> Variants { get; set; }

    /// <summary>
    /// When the current entry is edited, the edited version will be stored here until it is published
    /// TODO: Application rule: A name with a pending publish cannot be edited
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
        Syllables = new List<string>();
        FamousPeople = new List<string>();
        Variants = new List<string>();
        Duplicates = new List<NameEntry>();
        Feedbacks = new List<Feedback>();
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

    public int CompareTo(NameEntry other)
    {
        return Name.CompareTo(other.Name);
    }
}