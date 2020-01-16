using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;
using WebApp1.Core;
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
    private readonly IUnitOfWork unitOfWork;

    public RolesController(IMapper mapper,
                            RoleManager<ApplicationRole> roleManager,
                            UserManager<ApplicationUser> userManager,
                            IUnitOfWork unitOfWork)
    {
      this.userManager = userManager;
      this.unitOfWork = unitOfWork;
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
          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new ResponseObjectResulrResource(
            $"New role with name ({roleResource.RoleName}) was created",
            null
          ));
        }
        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return new BadRequestObjectResult(
                    new ResponseObjectResulrResource("Invalid request",
                                                      ModelState.Values.Select(
                                                        e => e.Errors[0].ErrorMessage
                                                      ).ToList()));
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
          return new NotFoundObjectResult(new ResponseObjectResulrResource(
            $"Role with Id: ({createRoleResource.Id}) cannot be found",
            null
          ));
        }
        else
        {
          this.mapper.Map<CreateRoleResource, IdentityRole>(createRoleResource, role);
          var result = await roleManager.UpdateAsync(role);

          if (result.Succeeded)
          {
            await this.unitOfWork.CompleteAsync();

            return new OkObjectResult(new ResponseObjectResulrResource(
              $"Role with Id: ({createRoleResource.Id}) was updated",
              null
            ));
          }

          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
        }
      }

      return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Invalid request",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await roleManager.FindByIdAsync(id);

      if (role == null)
      {
        return new NotFoundObjectResult(new ResponseObjectResulrResource(
          $"Role with Id: ({id}) cannot be found",
          null
        ));
      }

      try
      {
        var result = await roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new ResponseObjectResulrResource(
            $"Role ({role.Name}) deleted",
            null
          ));
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }

        return new BadRequestObjectResult(ModelState);
      }
      catch (DbUpdateException e)
      {
        return new BadRequestObjectResult(new ResponseObjectResulrResource(
          $"({role.Name}) is in use",
          null
        ));
      }
    }
  }
}