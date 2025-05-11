using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.RemoteData.RemoteDatabase.DAO
{
    public class AttendanceInputModel
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
        public int LessonNumber { get; set; }
        public bool IsAttendance { get; set; }
    }
}
