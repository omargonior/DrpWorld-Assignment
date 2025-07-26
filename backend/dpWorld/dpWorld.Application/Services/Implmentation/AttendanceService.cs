using AutoMapper;
using dpWorld.Application.DTOs.AttendanceDTO;
using dpWorld.Application.Services.Abstruct;
using dpWorld.Data.Models;
using dpWorld.Infrastructure.Repos.Abstruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.Services.Implmentation
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public AttendanceService(
            IAttendanceRepository attendanceRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _attendanceRepo = attendanceRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<bool> CheckInAsync(string userId)
        {
            var now = DateTime.Now;

            // Check time window (7:30 AM to 9:00 AM UTC)
            var start = DateTime.Now.Date.AddHours(7.5); // 7:30 AM //should use UtcNow but i use DateTime.Now for testing purposes
            var end = DateTime.Now.Date.AddHours(9);     // 9:00 AM //should use UtcNow but i use DateTime.Now for testing purposes

            if (now < start || now > end)
                return false;

            var existing = await _attendanceRepo.GetTodayAttendanceAsync(userId);
            if (existing != null)
                return false;

            var attendance = new Attendance
            {
                UserId = userId,
                CheckInTime = now
            };

            await _attendanceRepo.AddAsync(attendance);
            return true;
        }

        public async Task<List<AttendanceDto>> GetUserHistoryAsync(string userId)
        {
            var records = await _attendanceRepo.GetByUserIdAsync(userId);
            return _mapper.Map<List<AttendanceDto>>(records);
        }

        public async Task<List<AttendanceSummaryDto>> GetWeeklySummaryAsync()
        {
            var allUsers = await _userRepo.GetAllAsync();
            var summaries = new List<AttendanceSummaryDto>();

            foreach (var user in allUsers)
            {
                var history = await _attendanceRepo.GetByUserIdAsync(user.Id);
                var weekly = history
                    .Where(a => a.CheckInTime >= DateTime.Now.Date.AddDays(-7))
                    .ToList();

                // Find today's check-in
                var today = DateTime.Now.Date;//should use UtcNow but i use DateTime.Now for testing purposes
                var todayCheckIn = history.FirstOrDefault(a => a.CheckInTime.Date == today)?.CheckInTime;
                var todayAttendance = weekly.FirstOrDefault(a => a.CheckInTime.Date == today);
                int hoursWorkedToday = 0;
                if (todayAttendance != null)
                {
                    // Calculate hours from check-in to now, capped at 8
                    var checkIn = todayAttendance.CheckInTime;
                    var now = DateTime.Now;//should use UtcNow but i use DateTime.Now for testing purposes
                    var diff = (now - checkIn).TotalHours;
                    if (diff < 0) diff = 0;
                    if (diff > 8) diff = 8;
                    hoursWorkedToday = (int)Math.Floor(diff);
                }

                summaries.Add(new AttendanceSummaryDto
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    TotalDaysCheckedIn = weekly.Count,
                    TotalHours = ((weekly.Count - (todayAttendance != null ? 1 : 0)) * 8) + hoursWorkedToday,
                    TodayCheckIn = todayCheckIn
                });
            }

            return summaries;
        }
    }
}
