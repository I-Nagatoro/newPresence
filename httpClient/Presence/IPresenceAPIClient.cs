using httpClient.Group.Models;
namespace httpClient.Presence;

public interface IPresenceAPIClient
{
    Task<PresenceResponse?> GetPresenceAsync(int groupId, DateOnly startDate, DateOnly endDate);
    Task<bool> DeletePresenceRecords(string date, int lessonNumder, Guid userGuid);
}