namespace Core.Dto.Request
{
    public record UpdateUserDto (string? Username, string? Password, string[]? Roles, string? UpdatedBy)
    {

    }
}
