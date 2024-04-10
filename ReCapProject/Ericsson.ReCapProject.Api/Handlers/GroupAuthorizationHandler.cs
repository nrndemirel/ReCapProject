using Ericsson.ReCapProject.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Ericsson.ReCapProject.Api.Handlers
{
    public class GroupAuthorizationHandler : AuthorizationHandler<GroupAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupAuthorizationRequirement requirement)
        {
            if (context.User.Claims.Any(x => x.Type == "groups" && x.Value == requirement.GroupId))
                context.Succeed(requirement);

            var result = Task.CompletedTask;
            return result;
        }
    }
}
