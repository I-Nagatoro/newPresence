using data.RemoteData.RemoteDatabase.DAO;
using httpClient.Group.Models;

namespace httpClient.Group;

public interface IGroupAPIClient
{
    Task<List<GroupDAO>> GetGroupsAsync();
    Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync();
    Task RemoveAllUsersFromGroup(int groupId);
}