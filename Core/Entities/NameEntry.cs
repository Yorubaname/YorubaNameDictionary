using YorubaOrganization.Core.Entities;

namespace Core.Entities;

public class NameEntry : DictionaryEntry<NameEntry>
{
    public string Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }

    // I leave the following fields here because we will need to copy the values over into the new type during migration.
    // They should be deprecated eventually.
    public List<string> FamousPeople { get; set; }

    protected override void InitializeLists()
    {
        base.InitializeLists();
        FamousPeople = [];
    }
}