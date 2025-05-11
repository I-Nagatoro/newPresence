using data.Domain.UseCase;
using data.RemoteData.RemoteDatabase.DAO;

namespace ui
{
    public class PresenceConsoleUI
    {
        private readonly PresenceUseCase _presenceUseCase;

        public PresenceConsoleUI(PresenceUseCase presenceUseCase)
        {
            _presenceUseCase = presenceUseCase;
        }

        public void ExportAttendanceToExcel()
        {
            try
            {
                _presenceUseCase.ExportAttendanceToExcel();
                Console.WriteLine("Данные посещаемости успешно экспортированы в Excel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте посещаемости: {ex.Message}");
            }
        }



        public void GeneratePresenceForDay(DateTime date, int groupId, int firstLesson, int lastLesson)
        {
            try
            {
                _presenceUseCase.GeneratePresenceDaily(firstLesson, lastLesson, groupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации посещаемости: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public void GeneratePresenceForWeek(DateTime date, int groupId, int firstLesson, int lastLesson)
        {
            try
            {
                _presenceUseCase.GenerateWeeklyPresence(firstLesson, lastLesson, groupId, DateOnly.FromDateTime(date));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации посещаемости: {ex.Message}");
            }
        }

        public void DisplayPresence(DateTime date, int groupId)
        {
            try
            {
                List<PresenceDAO> presences = _presenceUseCase.GetPresenceByDateAndGroup(DateOnly.FromDateTime(date), DateOnly.FromDateTime(date), groupId);

                if (presences == null || presences.Count == 0)
                {
                    Console.WriteLine("Посещаемость на выбранную дату отсутствует.");
                    return;
                }

                var sortedPresences = presences.OrderBy(p => p.LessonNumber)
                                                .ThenBy(p => p.UserId);

                Console.WriteLine($"\nПосещаемость на {date.ToShortDateString()} для группы с ID {groupId}:");
                Console.WriteLine("---------------------------------------------");

                int previousLessonNumber = -1;
                foreach (var presence in sortedPresences)
                {
                    if (previousLessonNumber != presence.LessonNumber)
                    {
                        Console.WriteLine("---------------------------------------------");
                        previousLessonNumber = presence.LessonNumber;
                    }
                    string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
                    Console.WriteLine($"Пользователь ID: {presence.UserId}, Занятие {presence.LessonNumber}: {status}");
                }
                Console.WriteLine("---------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выводе посещаемости: {ex.Message}");
            }

        }

        public void MarkUserAbsent(DateTime date, int groupId, int userId, int firstLesson, int lastLesson)
        {
            bool check = _presenceUseCase.MarkUserAbsentForLessons(userId, groupId, firstLesson, lastLesson, DateOnly.FromDateTime(date));
            if (check)
            {
                Console.WriteLine("Пользователь отмечен как осутсвующий");
            }
            else
            {
                Console.WriteLine($"Посещаемость для пользователя ID: {userId} на дату {date.ToShortDateString()}" +
                  $" с {firstLesson} по {lastLesson} уроки не найдена.");
            }
        }

        public void DisplayGeneralPresence(int groupId)
        {
            var statistics = _presenceUseCase.GetGeneralPresence(groupId);
            Console.WriteLine($"Человек в группе: {statistics.UserCount}, " +
                              $"Количество проведённых занятий: {statistics.TotalLessons}, " +
                              $"Общий процент посещаемости группы: {statistics.AttendancePercentage}%");

            foreach (var user in statistics.UserAttendanceDetails)
            {
                Console.ForegroundColor = user.AttendanceRate < 40 ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine($"ID Пользователя: {user.UserId}, Посетил: {user.Attended}, " +
                                  $"Пропустил: {user.Missed}, Процент посещаемости: {user.AttendanceRate}%");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }



        public void DisplayAllPresenceByGroup(int groupId)
        {
            try
            {
                var presences = _presenceUseCase.GetAllPresenceByGroup(groupId);

                if (presences == null || presences.Count == 0)
                {
                    Console.WriteLine($"Посещаемость для группы с ID {groupId} отсутствует.");
                    return;
                }

                var groupedPresences = presences.GroupBy(p => p.Date);

                foreach (var group in groupedPresences)
                {
                    Console.WriteLine("===================================================");
                    Console.WriteLine($"Дата: {group.Key.ToString("dd.MM.yyyy")}");
                    Console.WriteLine("===================================================");

                    var groupedByLesson = group.GroupBy(p => p.LessonNumber);

                    foreach (var lessonGroup in groupedByLesson)
                    {
                        Console.WriteLine($"Занятие {lessonGroup.Key}:");

                        var userIds = new HashSet<int>();

                        foreach (var presence in lessonGroup)
                        {
                            if (userIds.Add(presence.UserId))
                            {
                                string status = presence.IsAttendance ? "Присутствует" : "Отсутствует";
                                Console.WriteLine($"Пользователь ID: {presence.UserId}, Статус: {status}");
                            }
                        }

                        Console.WriteLine("---------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выводе посещаемости: {ex.Message}");
            }
        }


    }
}