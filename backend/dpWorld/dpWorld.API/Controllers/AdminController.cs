using dpWorld.Application.DTOs;
using dpWorld.Application.DTOs.UserDTO;
using dpWorld.Application;
using dpWorld.Application.Services.Abstruct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dpWorld.Shared.Common.Pagination;

namespace dpWorld.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttendanceService _attendanceService; // Add this for attendance operations

        public AdminController(IUserService userService, IAttendanceService attendanceService)
        {
            _userService = userService;
            _attendanceService = attendanceService; 
        }

        // ... existing code ...

        [HttpGet("attendance-summary")]
        public async Task<IActionResult> GetAttendanceSummary()
        {
            var summary = await _attendanceService.GetWeeklySummaryAsync();
            return Ok(summary);
        }


       

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        // ... existing code ...
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            var (result, errors) = await _userService.CreateWithErrorsAsync(dto, "Employee");
            if (result)
                return Ok(new { message = "User created." });
            return BadRequest(errors ?? "Failed to create user.");
        }
        // ... existing code ...


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return result ?  Ok(new { message = "User updated." }) : NotFound("User not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            return result ? Ok(new { message = "User deleted." }) : NotFound("User not found.");
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] UserQueryParameters parameters)
        {
            var result = await _userService.GetPagedUsersAsync(parameters);
            return Ok(result);
        }

    }
}
