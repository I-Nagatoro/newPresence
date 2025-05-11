using data.RemoteData.RemoteDataBase;
using data.Repository;
using data.Domain.UseCase;
using System;
using System.Text;
using data.RemoteData.RemoteDatabase.DAO;

namespace ui
{
    public class UserConsoleUI
    {
        private readonly UserUseCase _userUseCase;
        private readonly GroupUseCase _groupUseCase;

        public UserConsoleUI(UserUseCase userUseCase, GroupUseCase groupUseCase)
        {
            _userUseCase = userUseCase;
            _groupUseCase = groupUseCase;
        }

        public void DisplayAllUsers()
        {
            Console.WriteLine("\n=== Список всех пользователей ===");
            StringBuilder userOutput = new StringBuilder();


            var users = _userUseCase.GetAllUsers();

            if (users == null || !users.Any())
            {
                Console.WriteLine("Нет пользователей для отображения.");
                return;
            }

            var groups = _groupUseCase.GetAllGroups();


            foreach (var user in users)
            {
                var group = groups?.FirstOrDefault(g => g.Id == user.GroupId); 
                string groupName = group != null ? group.Name : $"Группа {user.GroupId} не найдена";

                userOutput.AppendLine($"{user.UserId}\t{user.FIO}\t{groupName}");
            }

            Console.WriteLine(userOutput);
            Console.WriteLine("===============================\n");
        }



        public void RemoveUserById(int userId)
        {
            string output = _userUseCase.RemoveUserById(userId) ? "Пользователь удален" : "Пользователь не найден";
            Console.WriteLine($"\n{output}\n");
        }

        public void UpdateUserById(int userId)
        {
            try
            {
                var user = _userUseCase.FindUserById(userId);


                Console.WriteLine($"Текущие данные: {user.FIO}");
                Console.Write("\nВведите новое ФИО: ");
                string newFIO = Console.ReadLine();
                Console.Write("\nВведите новый ID группы (или оставьте такой же): ");
                int GroupId = int.Parse(Console.ReadLine());
                _userUseCase.UpdateUser(userId, newFIO, GroupId);

                Console.WriteLine("\nПользователь обновлен.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\n");
            }
        }

        public void FindUserById(int userId)
        {
            UserDAO? user = _userUseCase.FindUserById(userId);
            if (user != null)
            {
                Console.WriteLine($"\nПользователь найден: ID: {user.UserId}, ФИО: {user.FIO}, Группа: {user.Group.Name}\n");
            }
            else
            {
                Console.WriteLine("\nПользователь не найден.\n");
            }
        }
    }
}
