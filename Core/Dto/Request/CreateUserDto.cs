using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Response
{
    public record CreateUserDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        [Required]
        public string[] Roles { get; set; }
    }
}
