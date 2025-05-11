namespace httpClient.Group.Models;

public class ClientDeleteUsersRequest
{
    public List<int> UsersId { get; set; } = new();
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
        
    public int GroupId { get; set; }
}