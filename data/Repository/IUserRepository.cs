using data.RemoteData.RemoteDatabase.DAO;

public interface IUserRepository
{
    Task<bool> RemoveUserByIdAsync(int userId);
    Task<UserDAO> UpdateUserAsync(UserDAO user);
    public Task<UserDAO> AddUserAsync(UserDAO user);
    public UserDAO AddUser(UserDAO user);
    Task<List<UserDAO>> GetAllUsersAsync(bool trackEntities = false);
    Task<List<UserDAO>> GetUserNamesAsync();
    List<UserDAO> GetAllUsers();
    bool RemoveUserById(int userId);
    List<UserDAO> GetUserNames();
    UserDAO? GetUserById(int userId);
    public List<UserDAO> GetUsersByGroupId(int groupId);
    
}