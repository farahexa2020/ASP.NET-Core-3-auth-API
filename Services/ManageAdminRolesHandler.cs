using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WebApp1.Core.Claims;
using WebApp1.Core.Models;

namespace WebApp1.Services
{
  public class ManageAdminRolesHandler : AuthorizationHandler<ManageAdminRolesRequiremnet>
  {
    public IHttpContextAccessor httpContextAccessor { get; set; }
    private readonly IServiceProvider serviceProvider;
    public ManageAdminRolesHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base()
    {
      this.serviceProvider = serviceProvider;
      this.httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesRequiremnet requirement)
    {
      try
      {
        string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var editedUserId = this.httpContextAccessor.HttpContext.Request.Query["userId"];
        var roleId = this.httpContextAccessor.HttpContext.Request.Query["roleId"];

        bool isEditedUserAdmin;
        bool isRoleAdmin;
        using (var scope = this.serviceProvider.CreateScope())
        {
          var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
          var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

          var user = userManager.FindByIdAsync(editedUserId).Result;
          isEditedUserAdmin = userManager.IsInRoleAsync(user, Roles.Admin.ToString()).Result;

          var adminRoleId = roleManager.GetRoleIdAsync(roleManager.FindByNameAsync(Roles.Admin.ToString()).Result).Result;
          isRoleAdmin = roleId.ToString().ToLower() == adminRoleId.ToString().ToLower();
        }

        if (
            (context.User.HasClaim(ClaimTypes.Role, AdminClaimValues.SuperAdmin.ToString())) ||
            (context.User.IsInRole(Roles.Admin.ToString()) && !isEditedUserAdmin && !isRoleAdmin)
          )
        {
          context.Succeed(requirement);
        }
        // }
        // var editedUserId = this.httpContextAccessor.HttpContext.Request.Query["id"];

        // if (context.User.IsInRole(Roles.Admin.ToString()) && loggedInUserId.ToLower() != editedUserId.ToString().ToLower())
        // {
        //   context.Succeed(requirement);
        // }
      }
      catch (Exception e)
      {
        return Task.CompletedTask;
      }


      return Task.CompletedTask;
    }
  }
}