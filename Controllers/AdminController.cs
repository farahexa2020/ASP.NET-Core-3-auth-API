using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AdminController : Controller
  {
    private readonly IMapper mapper;
    private readonly RoleManager<IdentityRole> roleManager;
    public AdminController(IMapper mapper, RoleManager<IdentityRole> roleManager)
    {
      this.mapper = mapper;
      this.roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleResource roleResource)
    {
      if (ModelState.IsValid)
      {
        var identityRole = this.mapper.Map<CreateRoleResource, IdentityRole>(roleResource);

        var result = await roleManager.CreateAsync(identityRole);
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
      return new OkObjectResult(roles);
    }

    // [HttpPost("{id}/update")]
    // public async Task<IActionResult> UpdateRole(string id)
    [HttpPost("update")]
    public async Task<IActionResult> UpdateRole(CreateRoleResource createRoleResource)
    {
      var role = await roleManager.FindByIdAsync(createRoleResource.Id);

      if (role == null)
      {
        return new BadRequestObjectResult($"Role with Id: ({createRoleResource.Id}) cannot be found");
      }
      else
      {
        role = this.mapper.Map<CreateRoleResource, IdentityRole>(createRoleResource);
        var result = await roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
          return new OkObjectResult("Role updated");
        }
        else
        {
          return new BadRequestObjectResult("Failed to update role");
        }
      }
    }
  }
}