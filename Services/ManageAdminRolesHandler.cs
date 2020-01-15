using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp1.Core.Models;

namespace WebApp1.Services
{
  public class ManageAdminRolesHandler : AuthorizationHandler<ManageAdminRolesRequiremnet>
  {
    public IHttpContextAccessor HttpContextAccessor { get; set; }
    public ManageAdminRolesHandler(IHttpContextAccessor httpContextAccessor) : base()
    {
      this.HttpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesRequiremnet requirement)
    {
      try
      {
        string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var routeValues = this.HttpContextAccessor.HttpContext.Request.RouteValues;
        object editedUserId;
        routeValues.TryGetValue("id", out editedUserId);

        if (context.User.IsInRole(Roles.Admin.ToString()) && loggedInUserId.ToLower() != editedUserId.ToString().ToLower())
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