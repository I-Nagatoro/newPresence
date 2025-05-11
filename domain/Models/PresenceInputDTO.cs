namespace domain.Models;

public class PresenceInputDTO
{
    public DateOnly Date { get; set; }
    public int LessonNumber { get; set; }
    public int Student { get; set; }
    public bool Status { get; set; }
    public string TypeAttendance => Status ? "Присутствовал" : "Отсутствовал";
}