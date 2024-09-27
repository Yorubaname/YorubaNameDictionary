namespace YorubaOrganization.Core.Entities;

public class User : BaseEntity
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<string> Roles { get; set; }

    public User()
    {
        Roles = new List<string>();
    }
}
