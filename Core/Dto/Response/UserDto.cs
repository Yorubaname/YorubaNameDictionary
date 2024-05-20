using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Response
{
    public record UserDto
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string[] Roles { get; set; }
    }
}
