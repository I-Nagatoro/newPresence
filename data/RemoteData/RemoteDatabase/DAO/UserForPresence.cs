using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.domain.Models
{
    public class UsersForPresence
    {
        public string FIO { get; set; }
        public int LessonNumber { get; set; }
        public DateOnly Date { get; set; }
        public bool isAttendance { get; set; }
    }
}
