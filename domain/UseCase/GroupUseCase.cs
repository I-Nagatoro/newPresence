using data.RemoteData.RemoteDataBase.DAO;
using data.Repository;
using data.domain.Models;
using data.RemoteData.RemoteDatabase.DAO;

namespace data.Domain.UseCase
{
    public class GroupUseCase
    {
        private readonly IGroupRepository _SQLGroupRepositoryImpl;

        public GroupUseCase(IGroupRepository SQlGroupRepositoryImpl)
        {
            _SQLGroupRepositoryImpl = SQlGroupRepositoryImpl;
        }

        private void ValidateGroupName(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("Имя группы не может быть пустым.");
            }
        }

        public void RemoveAllStudentsFromGroup(int groupId)
        {
            var existingGroup = ValidateGroupExistence(groupId);
            _SQLGroupRepositoryImpl.RemoveAllStudentsFromGroup(existingGroup.Id);
        }

        private GroupDAO ValidateGroupExistence(int groupId)
        {
            var existingGroup = _SQLGroupRepositoryImpl.GetAllGroups()
                .FirstOrDefault(g => g.Id == groupId);

            if (existingGroup == null)
            {
                throw new ArgumentException("Группа не найдена.");
            }

            return existingGroup;
        }


        public List<GroupDAO> GetAllGroups()
        {
            return _SQLGroupRepositoryImpl.GetAllGroups();
        }

        public string FindGroupById(int IdGroup)
        {
            string groups = _SQLGroupRepositoryImpl.GetGroupById(IdGroup).Name;

            return groups;
        }



        public void AddGroup(string groupName)
        {
            ValidateGroupName(groupName);

            _SQLGroupRepositoryImpl.AddGroup(groupName);
        }


        public void UpdateGroup(int groupId, string newGroupName)
        {
            ValidateGroupName(newGroupName);
            var existingGroup = ValidateGroupExistence(groupId);

            existingGroup.Name = newGroupName;
            _SQLGroupRepositoryImpl.UpdateGroupById(groupId, existingGroup);
        }

        public void DeleteGroupById(int groupId)
        {
            var existingGroup = ValidateGroupExistence(groupId);
            _SQLGroupRepositoryImpl.DeleteGroupById(groupId);
        }
    }
}