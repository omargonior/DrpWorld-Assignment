using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.DTOs.TokenDTO
{
    public class TokenDto
    {
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
