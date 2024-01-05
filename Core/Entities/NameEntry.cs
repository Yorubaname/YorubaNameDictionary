using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class NameEntry : Entity , IComparable<NameEntry>
{
    [Required]
    [StringLength(255)] // Adjust string length constraint as needed
    public string Name { get; set; }

    public string? Pronunciation { get; set; }
    // TODO: Clarify need to retain this field; Only 5 records exist in database at the moment
    public string? IpaNotation { get; set; }
    public string? Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }
    public string? Morphology { get; set; }
    public string? Media { get; set; }
    public State? State { get; set; }

    // Note: Did not migrate TonalMark, Tags, InOtherLanguages intentionally since all values are null in the database (no admin boxes for them)
    public List<Etymology>? Etymology { get; set; }
    public List<EmbeddedVideo>? Videos { get; set; }
    public List<GeoLocation>? GeoLocation { get; set; }
    
    // TODO: Previously comma separated
    public List<string> FamousPeople { get; set; }

    // TODO: Previously hyphen separated
    public List<string> Syllables { get; set; }
    
    // TODO: Previously comma separated 
    public List<string> Variants { get; set; }

    public NameEntry(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        Etymology = new List<Etymology>();
        Videos = new List<EmbeddedVideo>();
        Syllables = new List<string>();
        FamousPeople = new List<string>();
        Variants = new List<string>();
    }

    // TODO: Intentionally removed update method

    public int CompareTo(NameEntry other)
    {
        return Name.CompareTo(other.Name);
    }
}