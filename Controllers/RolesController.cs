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
using WebApp1.Controllers.Resources.ApiResponse;
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

          return new OkObjectResult(new CreatedResource(
            $"New role with name ({roleResource.RoleName}) was created"
          ));
        }
        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpGet]
    public IActionResult GetRoles()
    {
      var roles = this.roleManager.Roles;

      var respone = this.mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleResource>>(roles);

      return new OkObjectResult(new OkResource(
        "All Roles",
        respone
      ));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(CreateRoleResource createRoleResource)
    {
      if (ModelState.IsValid)
      {
        var role = await roleManager.FindByIdAsync(createRoleResource.Id);

        if (role == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            $"Role with Id: ({createRoleResource.Id}) cannot be found"
          ));
        }
        else
        {
          this.mapper.Map<CreateRoleResource, IdentityRole>(createRoleResource, role);
          var result = await roleManager.UpdateAsync(role);

          if (result.Succeeded)
          {
            await this.unitOfWork.CompleteAsync();

            return new OkObjectResult(new OkResource(
              $"Role with Id: ({createRoleResource.Id}) was updated"
            ));
          }

          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      var role = await roleManager.FindByIdAsync(id);

      if (role == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
          $"Role with Id: ({id}) cannot be found"
        ));
      }

      try
      {
        var result = await roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new OkResource(
            $"Role ({role.Name}) deleted"
          ));
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }

        return new BadRequestObjectResult(new BadRequestResource(
          "Invalid request",
          ModelState.Keys
          .SelectMany(key => ModelState[key].Errors.Select
                        (x => new ValidationErrorResource(key, x.ErrorMessage)))
          .ToList()
        ));
      }
      catch (DbUpdateException e)
      {
        return new BadRequestObjectResult(new BadRequestResource(
          $"({role.Name}) is in use"
        ));
      }
    }
  }
}