
    using dpWorld.Application;
    using global::dpWorld.Application.Services.Abstruct;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace dpWorld.API.Controllers
{
    [ApiController]
    [Route("api/employee")]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAttendanceService _attendanceService;

        public EmployeeController(IUserService userService, IAttendanceService attendanceService)
        {
            _userService = userService;
            _attendanceService = attendanceService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetByIdAsync(userId);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _attendanceService.CheckInAsync(userId);
            if (!success)
                return BadRequest("You have already checked in today or it is outside the check-in window (7:30 AM - 9:00 AM UTC).");
            return Ok(new { message = "Checked in successfuly." });
        }

        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendanceHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var records = await _attendanceService.GetUserHistoryAsync(userId);
            return Ok(records);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("signature")]
        public async Task<IActionResult> AddSignature([FromForm] IFormFile signature)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using var ms = new MemoryStream();
            await signature.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            await _userService.UpdateSignatureAsync(userId, base64);
            return Ok();
        }

    }
}



