using dpWorld.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Data.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public DateTime CheckInTime { get; set; }

        public ApplicationUser User { get; set; } = default!;
    }
}
