using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.DTOs.AttendanceDTO
{
    public class AttendanceSummaryDto
    {
        public string UserId { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public int TotalDaysCheckedIn { get; set; }
        public int TotalHours { get; set; }
        public DateTime? TodayCheckIn { get; set; } 

    }
}
