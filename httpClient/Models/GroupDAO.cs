namespace httpClient.Group.Models;

public class GroupDAO
{
    public required int ID{get; set; }
    public required string Name{get; set; }

    public static GroupDAO Parse(string input){
        string[] words = input.Split(" ");
        return new GroupDAO{ID = Convert.ToInt32(words[0]), Name = words[1]};
    }
}