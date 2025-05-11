using System;

namespace httpClient.Group.Models;

    public class PresenceDAO
    {
        public DateTime Date { get; set; }
        public int LessonNumber { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public bool IsAttendance { get; set; }
    }

    public class CreatePresenceRequest
    {
        public DateTime Date { get; set; }
        public int LessonNumber { get; set; }
        public int StudentId { get; set; }
        public bool TypeAttendance { get; set; }
        public int GroupId { get; set; }
    }

    public class UpdatePresenceRequest
    {
        public DateTime Date { get; set; }
        public int LessonNumber { get; set; }
        public int StudentId { get; set; }
        public bool NewTypeAttendance { get; set; }
        public int GroupId { get; set; }
    }

    public class AddGroupRequest
    {
        public string GroupName { get; set; } = default!;
    }

    public class StudentsRequest
    {
        public List<int> Students { get; set; } = new();
    }