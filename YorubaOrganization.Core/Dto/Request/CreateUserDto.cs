namespace YorubaOrganization.Core.Dto.Request
{
    public record CreateUserDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        public string[] Roles { get; set; }

        public string? CreatedBy { get; set; }
    }
}
