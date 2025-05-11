namespace httpClient.Group.Models;

public class GroupDTO
{
    public required int ID{get; set; }
    public required string Name{get; set; }

    public static GroupDTO Parse(string input){
        string[] words = input.Split(" ");
        return new GroupDTO{ID = Convert.ToInt32(words[0]), Name = words[1]};
    }
}