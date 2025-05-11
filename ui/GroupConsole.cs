using data.Domain.UseCase;
using System.Text;

namespace ui
{
    public class GroupConsoleUI
    {
        private readonly GroupUseCase _groupUseCase;

        public GroupConsoleUI(GroupUseCase groupUseCase)
        {
            _groupUseCase = groupUseCase;
        }

        public void FindGroupById(int IdGroup)
        {
            var groups = _groupUseCase.FindGroupById(IdGroup);
            Console.WriteLine(groups);
        }

        public void DisplayAllGroups()
        {
            Console.WriteLine("\n=== Список всех групп ===");
            StringBuilder groupOutput = new StringBuilder();

            foreach (var group in _groupUseCase.GetAllGroups())
            {
                groupOutput.AppendLine($"{group.Id}\t{group.Name}");
            }

            Console.WriteLine(groupOutput);
            Console.WriteLine("===========================\n");
        }

        public void AddGroup(string groupName)
        {
            try
            {
                _groupUseCase.AddGroup(groupName);
                Console.WriteLine($"\nГруппа {groupName} добавлена.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\n");
            }
        }

        public void UpdateGroupName(int groupId, string newGroupName)
        {
            _groupUseCase.UpdateGroup(groupId, newGroupName);
            Console.WriteLine($"\nНазвание группы с ID {groupId} изменено на {newGroupName}.\n");
        }
    }
}
