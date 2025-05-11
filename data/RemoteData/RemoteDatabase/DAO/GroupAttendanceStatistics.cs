using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.RemoteData.RemoteDatabase.DAO
{
    public class GroupAttendanceStatistics
    {
        public int UserCount { get; set; }
        public int TotalLessons { get; set; }
        public double AttendancePercentage { get; set; }
        public List<UserAttendance> UserAttendanceDetails { get; set; } = new List<UserAttendance>();
    }
}
