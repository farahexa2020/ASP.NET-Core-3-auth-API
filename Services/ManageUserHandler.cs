using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using WebApp1.Constants;

namespace WebApp1.Services
{
  public class ManageUserHandler : AuthorizationHandler<ManageUserRequirements>
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    public ManageUserHandler(IHttpContextAccessor httpContextAccessor)
    {
      this.httpContextAccessor = httpContextAccessor;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageUserRequirements requirement)
    {
      try
      {
        string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var routeValues = this.httpContextAccessor.HttpContext.Request.RouteValues;
        object editedUserId;
        routeValues.TryGetValue("userId", out editedUserId);

        if (context.User.IsInRole(RolesEnum.User.ToString()) && loggedInUserId.ToLower() == editedUserId.ToString().ToLower())
        {
          context.Succeed(requirement);
        }
      }
      catch (Exception e)
      {
        return Task.CompletedTask;
      }


      return Task.CompletedTask;
    }
  }
}