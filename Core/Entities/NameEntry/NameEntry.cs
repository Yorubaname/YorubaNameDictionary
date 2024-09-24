using Core.Core.Entities;

namespace Core.Entities.NameEntry;

public class NameEntry : DictionaryEntry<NameEntry>
{
    public string Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }

    // I leave the following fields here because we will need to copy the values over into the new type during migration.
    // They should be deprecated eventually.
    public List<string> Media { get; set; }
    public List<string> FamousPeople { get; set; }
    public List<string> Variants { get; set; }

    protected override void InitializeLists()
    {
        base.InitializeLists();
        Variants = [];
    }
}