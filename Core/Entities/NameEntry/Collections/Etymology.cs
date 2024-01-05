namespace Core.Entities.NameEntry.Collections;

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