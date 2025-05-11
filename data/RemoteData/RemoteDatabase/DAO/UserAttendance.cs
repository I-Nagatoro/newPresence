using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.RemoteData.RemoteDatabase.DAO
{
    public class UserAttendance
    {
        public int UserId { get; set; }
        public double Attended { get; set; }
        public double Missed { get; set; }
        public double AttendanceRate { get; set; }
    }
}
