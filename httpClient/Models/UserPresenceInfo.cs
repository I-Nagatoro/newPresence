namespace httpClient.Group.Models;
public class UserPresenceInfo
{
    public Guid Guid { get; set; }
    public string FIO { get; set; }
    public int LessonNumber { get; set; }
    public DateOnly Date { get; set; }
    public bool IsAttendance { get; set; }
}