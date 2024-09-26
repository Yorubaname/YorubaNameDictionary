using YorubaOrganization.Core.Entities;

namespace YorubaOrganization.Core.Entities.Partials;

public class Etymology : BaseEntity
{
    public string Part { get; set; }
    public string Meaning { get; set; }

    public Etymology(string part, string meaning)
    {
        Part = part;
        Meaning = meaning;
    }
}