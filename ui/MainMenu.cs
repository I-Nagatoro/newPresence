using System;
using System.Globalization;
using data.Domain.UseCase;
using ui;

namespace ui
{
    public class MainMenuUI
    {
        private readonly UserConsoleUI _userConsoleUI;
        private readonly GroupConsoleUI _groupConsoleUI;
        private readonly PresenceConsoleUI _presenceConsoleUI;

        public MainMenuUI(UserUseCase userUseCase, GroupUseCase groupUseCase, PresenceUseCase presenceUseCase)
        {
            _userConsoleUI = new UserConsoleUI(userUseCase, groupUseCase);
            _groupConsoleUI = new GroupConsoleUI(groupUseCase);
            _presenceConsoleUI = new PresenceConsoleUI(presenceUseCase);
        }

        public void Start()
        {
            while (true)
            {
                ShowNavigation();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        _userConsoleUI.DisplayAllUsers();
                        break;

                    case "2":
                        HandleMemberRemoval();
                        break;

                    case "3":
                        HandleMemberUpdate();
                        break;

                    case "4":
                        SearchMember();
                        break;

                    case "5":
                        _groupConsoleUI.DisplayAllGroups();
                        break;

                    case "6":
                        CreateNewTeam();
                        break;

                    case "7":
                        ModifyTeamName();
                        break;

                    case "8":
                        FindTeam();
                        break;

                    case "9":
                        CreateDailyAttendance();
                        break;

                    case "10":
                        CreateWeeklyAttendance();
                        break;

                    case "11":
                        ViewAttendanceRecords();
                        break;

                    case "12":
                        RecordAbsence();
                        break;

                    case "13":
                        ShowTeamAttendanceHistory();
                        break;

                    case "14":
                        DisplayAttendanceSummary();
                        break;

                    case "15":
                        GenerateExcelReport();
                        break;

                    case "0":
                        Console.WriteLine("Завершение работы...");
                        return;

                    default:
                        Console.WriteLine("Некорректный ввод, повторите попытку.");
                        break;
                }
                Console.WriteLine();
            }
        }

        private void ShowNavigation()
        {
            Console.WriteLine("\n=== Управление системой ===\n");
            
            Console.WriteLine("Управление участниками:");
            Console.WriteLine("1. Список всех участников");
            Console.WriteLine("2. Удалить участника");
            Console.WriteLine("3. Обновить данные участника");
            Console.WriteLine("4. Найти участника\n");

            Console.WriteLine("Управление группами:");
            Console.WriteLine("5. Показать все группы");
            Console.WriteLine("6. Создать новую группу");
            Console.WriteLine("7. Переименовать группу");
            Console.WriteLine("8. Найти группу\n");

            Console.WriteLine("Управление посещаемостью:");
            Console.WriteLine("9. Создать записи за день");
            Console.WriteLine("10. Создать записи за неделю");
            Console.WriteLine("11. Просмотр посещаемости");
            Console.WriteLine("12. Зарегистрировать отсутствие");
            Console.WriteLine("13. История посещаемости группы");
            Console.WriteLine("14. Статистика посещаемости");
            Console.WriteLine("15. Экспорт в Excel\n");

            Console.Write("Выберите действие: ");
        }

        private int GetValidNumberInput(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Ошибка формата ввода. Повторите попытку.");
                Console.Write(prompt);
            }
            return result;
        }

        private DateTime GetValidDateInput(string prompt)
        {
            DateTime date;
            Console.Write(prompt);
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Некорректный формат даты. Используйте дд.мм.гггг");
                Console.Write(prompt);
            }
            return date;
        }

        private void HandleMemberRemoval()
        {
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            _userConsoleUI.RemoveUserById(userId);
        }

        private void HandleMemberUpdate()
        {
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            _userConsoleUI.UpdateUserById(userId);
        }

        private void SearchMember()
        {
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            _userConsoleUI.FindUserById(userId);
        }

        private void CreateNewTeam()
        {
            Console.Write("Введите название группы: ");
            _groupConsoleUI.AddGroup(Console.ReadLine());
        }

        private void ModifyTeamName()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            Console.Write("Введите новое название: ");
            _groupConsoleUI.UpdateGroupName(groupId, Console.ReadLine());
        }

        private void FindTeam()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _groupConsoleUI.FindGroupById(groupId);
        }

        private void CreateDailyAttendance()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int startSession = GetValidNumberInput("Начальный номер занятия: ");
            int endSession = GetValidNumberInput("Конечный номер занятия: ");
            
            _presenceConsoleUI.GeneratePresenceForDay(DateTime.Today, groupId, startSession, endSession);
            Console.WriteLine("Записи за день созданы успешно.");
        }

        private void CreateWeeklyAttendance()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int startSession = GetValidNumberInput("Начальный номер занятия: ");
            int endSession = GetValidNumberInput("Конечный номер занятия: ");
            
            _presenceConsoleUI.GeneratePresenceForWeek(DateTime.Today, groupId, startSession, endSession);
            Console.WriteLine("Записи за неделю созданы успешно.");
        }

        private void ViewAttendanceRecords()
        {
            DateTime date = GetValidDateInput("Введите дату (дд.мм.гггг): ");
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _presenceConsoleUI.DisplayPresence(date, groupId);
        }

        private void RecordAbsence()
        {
            DateTime date = GetValidDateInput("Введите дату отсутствия (дд.мм.гггг): ");
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            int userId = GetValidNumberInput("Введите идентификатор участника: ");
            int startSession = GetValidNumberInput("Начальный номер занятия: ");
            int endSession = GetValidNumberInput("Конечный номер занятия: ");

            _presenceConsoleUI.MarkUserAbsent(date, groupId, userId, startSession, endSession);
        }

        private void ShowTeamAttendanceHistory()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _presenceConsoleUI.DisplayAllPresenceByGroup(groupId);
        }

        private void DisplayAttendanceSummary()
        {
            int groupId = GetValidNumberInput("Введите идентификатор группы: ");
            _presenceConsoleUI.DisplayGeneralPresence(groupId);
        }

        private void GenerateExcelReport()
        {
            _presenceConsoleUI.ExportAttendanceToExcel();
            Console.WriteLine("Отчёт успешно экспортирован.");
        }
    }
}