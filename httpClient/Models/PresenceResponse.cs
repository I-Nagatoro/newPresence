﻿namespace httpClient.Group.Models;

public class PresenceResponse
{
    public string GroupName { get; set; }
    public List<UserPresenceInfo> Users { get; set; }
}