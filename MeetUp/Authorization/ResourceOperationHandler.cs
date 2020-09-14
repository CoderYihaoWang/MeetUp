using MeetUp.Entity;
using Microsoft.AspNetCore.Authorization;
using NLog.Web.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetUp.Authorization
{
    public class ResourceOperationHandler : AuthorizationHandler<ResourceOperationRequirement, Meetup>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Meetup resource)
        {
            if (requirement.OperationType == OperationType.Create || requirement.OperationType == OperationType.Read)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (resource.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
