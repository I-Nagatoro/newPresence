using httpClient.Group.Models;
using Microsoft.Extensions.Logging;

namespace httpClient.Group;

public class GroupAPIClient : BaseAPIClient, IGroupAPIClient
{
    private const string BasePath = "Group";
    
    public GroupAPIClient(IHttpClientFactory httpClientFactory, ILogger<GroupAPIClient> logger) 
        : base(httpClientFactory, logger)
    {
    }

    public async Task<List<GroupDAO>> GetGroupsAsync()
    {
        return await GetAsync<List<GroupDAO>>(BasePath) ?? new List<GroupDAO>();
    }

    public async Task<List<GroupWithStudentDAO>> GetGroupsWithUsersAsync()
    {
        return await GetAsync<List<GroupWithStudentDAO>>("Admin") ?? new List<GroupWithStudentDAO>();
    }
}