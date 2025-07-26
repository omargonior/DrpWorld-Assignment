using AutoMapper;
using dpWorld.Application.DTOs;
using dpWorld.Application.DTOs.LoginDTO;
using dpWorld.Application.DTOs.TokenDTO;
using dpWorld.Data.Models;
using dpWorld.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dpWorld.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenGenerator tokenGenerator, IMapper mapper)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenGenerator.GenerateToken(user, roles);

            return Ok(new TokenDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}
