using Microsoft.AspNetCore.Authorization;

namespace Ericsson.ReCapProject.Api.Authorization
{
    public class GroupAuthorizationRequirement(string groupId) : IAuthorizationRequirement
    {
        public string GroupId { get; } = groupId;
    }
}
