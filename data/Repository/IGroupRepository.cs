using data.RemoteData.RemoteDatabase.DAO;
using data.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.Repository
{
    public interface IGroupRepository
    {
        List<GroupDAO> GetAllGroups();
        bool UpdateGroupById(int groupID, GroupDAO updatedGroup);
        GroupDAO GetGroupById(int groupID);
        bool AddGroup(string groupName);
        public void RemoveAllStudentsFromGroup(int groupId);
        public void AddStudentToGroup(int groupId, UserDAO student);
        public GroupDAO GetGroupByName(string groupName);
        public void DeleteGroupById(int groupId);

    }
}