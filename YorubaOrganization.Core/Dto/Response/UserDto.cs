namespace YorubaOrganization.Core.Dto.Response
{
    public record UserDto
    {
        public string Email { get; set; }
        public string? Username { get; set; }
        public string[] Roles { get; set; }
    }
}
