using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Authorize(Policy = "AdminPolicy")]
  [Route("api/[controller]")]
  [ApiController]
  public class RolesController : Controller
  {
    private readonly IMapper mapper;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    public RolesController(IMapper mapper, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
      this.userManager = userManager;
      this.mapper = mapper;
      this.roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleResource roleResource)
    {
      if (ModelState.IsValid)
      {
        var applicationRole = this.mapper.Map<CreateRoleResource, ApplicationRole>(roleResource);

        var result = await roleManager.CreateAsync(applicationRole);
        if (result.Succeeded)
        {
          return new OkObjectResult("New Role was Created");
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return new BadRequestObjectResult(ModelState);
    }

    [HttpGet]
    public IActionResult GetRoles()
    {
      var roles = this.roleManager.Roles;

      var respone = this.mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleResource>>(roles);

      return new OkObjectResult(respone);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(CreateRoleResource createRoleResource)
    {
      if (ModelState.IsValid)
      {
        var role = await roleManager.FindByIdAsync(createRoleResource.Id);

        if (role == null)
        {
          return new BadRequestObjectResult($"Role with Id: ({createRoleResource.Id}) cannot be found");
        }
        else
        {
          this.mapper.Map<CreateRoleResource, IdentityRole>(createRoleResource, role);
          var result = await roleManager.UpdateAsync(role);

          if (result.Succeeded)
          {
            return new OkObjectResult("Role updated");
          }

          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
        }
      }

      return new BadRequestObjectResult(ModelState);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await roleManager.FindByIdAsync(id);

      if (role == null)
      {
        return new BadRequestObjectResult($"Role with Id: ({id}) cannot be found");
      }

      try
      {
        var result = await roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
          return new OkObjectResult($"Role ({role.Name}) deleted");
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }

        return new BadRequestObjectResult(ModelState);
      }
      catch (DbUpdateException e)
      {
        return new BadRequestObjectResult($"({role.Name}) is in use");
      }
    }
  }
}