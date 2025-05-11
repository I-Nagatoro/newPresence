namespace httpClient.User;

public interface IUserAPIClient
{
    Task<bool> DeleteUserAsync(Guid userGuid);
    Task<bool> UpdateUserFioAsync(Guid userGuid, string fio);
    Task<bool> DeleteUsersByGroupIdAsync(int groupId);
    Task<bool> CreateUser(string fio, string groupName);
}