using dpWorld.Application.DTOs.AttendanceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Application.Services.Abstruct
{
    public interface IAttendanceService
    {
        Task<bool> CheckInAsync(string userId);
        Task<List<AttendanceDto>> GetUserHistoryAsync(string userId);
        Task<List<AttendanceSummaryDto>> GetWeeklySummaryAsync();
    }
}
