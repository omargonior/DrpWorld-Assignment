using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.DTOs.UserDTO
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public int Age { get; set; }
        public string? SignatureUrl { get; set; }
    }
}
