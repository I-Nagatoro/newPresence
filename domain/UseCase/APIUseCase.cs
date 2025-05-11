using data.RemoteData.RemoteDatabase.DAO;
using data.Repository;
using domain.Models;
using presence_api.Models;

namespace data.Domain.UseCase;

public class APIUseCase
{
    public readonly IUserRepository _userRepository;
    public readonly IGroupRepository _groupRepository;
    public readonly IPresenceRepository _presenceRepository;

    public APIUseCase(IUserRepository userRepository, IGroupRepository groupRepository,
        IPresenceRepository presenceRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _presenceRepository = presenceRepository;
    }

    public void AddGroup(GroupAPI groups)
    {
        if (string.IsNullOrWhiteSpace(groups.Name))
            throw new ArgumentException("Название группы не должно быть пустым");
        
        _groupRepository.AddGroup(groups.Name);
    }
    public void AddStudentsToExistingGroup(int groupId, List<string> students)
    {
        var group = _groupRepository.GetGroupById(groupId);
        if (group == null)
            throw new Exception($"Группа с ID {groupId} не найдена");

        foreach (var fio in students)
        {
            var newUser = new UserDAO
            {
                UserId = _userRepository.GetAllUsers().Select(u=>u.UserId).Max() + 1,
                FIO = fio,
                GroupId = groupId
            };
            _userRepository.AddUser(newUser);
        }
    }
}