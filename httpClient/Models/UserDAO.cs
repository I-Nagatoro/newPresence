namespace httpClient.Group.Models;

public class ClientDeleteUsersRequest
{
    public List<Guid> UsersGuid { get; set; } = new();
}

public class FIOUpdate
{
    public string Fio { get; set; }
}

public class ClientDeleteUsersByGroupId
{
    public int GroupId { get; set; }
}
public class CreateUserRequest
{
    public string Fio { get; set; }
        
    public string GroupName { get; set; }
}