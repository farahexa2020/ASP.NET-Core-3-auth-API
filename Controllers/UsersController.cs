using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
  public class UsersController : Controller
  {
    private readonly IMapper mapper;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public UsersController(IMapper mapper,
                            RoleManager<ApplicationRole> roleManager,
                            UserManager<ApplicationUser> userManager,
                            IUserRepository userRepository,
                            IUnitOfWork unitOfWork)
    {
      this.userManager = userManager;
      this.userRepository = userRepository;
      this.unitOfWork = unitOfWork;
      this.roleManager = roleManager;
      this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersAsync([FromQuery] UserQueryResource userQueryResource)
    {
      var userQuery = mapper.Map<UserQueryResource, UserQuery>(userQueryResource);

      var queryResult = await this.userRepository.GetUsers(userQuery);

      var response = mapper.Map<QueryResult<ApplicationUser>, QueryResultResource<UserResource>>(queryResult);

      return new OkObjectResult(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(string id)
    {
      var user = await this.userManager.Users
                                      .Where(u => u.Id == id)
                                      .Include(u => u.UserRoles)
                                      .ThenInclude(ur => ur.Role)
                                      .SingleOrDefaultAsync();

      var response = this.mapper.Map<ApplicationUser, UserResource>(user);

      return new OkObjectResult(response);
    }

    [Authorize(Policy = "EditRolePolicy")]
    [HttpPost("{id}/assignrole")]
    public async Task<IActionResult> AddUserToRoleAsync(string id, [FromQuery] string roleId)
    {
      var user = await this.userManager.FindByIdAsync(id);
      if (user == null)
      {
        return new BadRequestObjectResult("User not found");
      }

      var role = await roleManager.FindByIdAsync(roleId);
      if (role == null)
      {
        return new BadRequestObjectResult("Role not found");
      }

      if (await userManager.IsInRoleAsync(user, role.Name))
      {
        return new BadRequestObjectResult($"User is already added to role {role.Name}");
      }

      var result = await userManager.AddToRoleAsync(user, role.Name);
      if (result.Succeeded)
      {
        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult($"User ({user.FirstName}) was added to role ({role.Name})");
      }

      foreach (IdentityError error in result.Errors)
      {
        ModelState.AddModelError("", error.Description);
      }

      return new BadRequestObjectResult(ModelState);
    }

    [Authorize(Policy = "EditRolePolicy")]
    [HttpDelete("{id}/assignrole")]
    public async Task<IActionResult> RemoveUserFromRoleAsync(string id, [FromQuery] string roleId)
    {
      var user = await this.userManager.FindByIdAsync(id);
      if (user == null)
      {
        return new BadRequestObjectResult("User not found");
      }

      var role = await roleManager.FindByIdAsync(roleId);
      if (role == null)
      {
        return new BadRequestObjectResult("Role not found");
      }

      if (!await userManager.IsInRoleAsync(user, role.Name))
      {
        return new BadRequestObjectResult($"User is already not in role {role.Name}");
      }

      var result = await userManager.RemoveFromRoleAsync(user, role.Name);
      if (result.Succeeded)
      {
        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult($"User ({user.FirstName}) was removed from role ({role.Name})");
      }

      foreach (IdentityError error in result.Errors)
      {
        ModelState.AddModelError("", error.Description);
      }

      return new BadRequestObjectResult(ModelState);
    }
  }
}