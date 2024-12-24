using YorubaOrganization.Core.Entities;

namespace Core.Entities;

public class NameEntry : DictionaryEntry<NameEntry>
{
    public string Meaning { get; set; }
    public string? ExtendedMeaning { get; set; }
    public List<string> FamousPeople { get; set; }

    protected override void InitializeLists()
    {
        base.InitializeLists();
        FamousPeople = [];
    }
}