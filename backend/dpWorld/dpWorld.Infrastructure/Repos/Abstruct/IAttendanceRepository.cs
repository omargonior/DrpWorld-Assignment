using dpWorld.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Infrastructure.Repos.Abstruct
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetByUserIdAsync(string userId);
        Task<Attendance?> GetTodayAttendanceAsync(string userId);
        Task AddAsync(Attendance attendance);
    }
}
