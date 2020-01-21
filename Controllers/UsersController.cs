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
using WebApp1.Controllers.Resources.ApiResponse;
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

      return new OkObjectResult(new OkResource(
        "All Users",
        response
      ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(string id)
    {
      var user = await this.userRepository.GetUserById(id);

      var response = this.mapper.Map<ApplicationUser, UserResource>(user);

      return new OkObjectResult(new OkResource(
        $"User with Id ({id})",
        response
      ));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] RegisterUserResource registerUserResource)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByEmailAsync(registerUserResource.Email);
        if (user != null)
        {
          return new BadRequestObjectResult(new BadRequestResource(
            "User is already exist"
          ));
        }

        user = this.mapper.Map<RegisterUserResource, ApplicationUser>(registerUserResource);

        var role = await roleManager.FindByNameAsync(Roles.User.ToString());
        if (role == null)
        {
          return new BadRequestObjectResult(new BadRequestResource(
            "Role not found"
          ));
        }

        user.UserRoles.Add(new ApplicationUserRole() { RoleId = role.Id });

        user.EmailConfirmed = true;
        user.IsActive = true;
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.CreateAsync(user, registerUserResource.Password);
        if (result.Succeeded)
        {
          user = await this.userRepository.GetUserById(user.Id);
          var userRes = this.mapper.Map<ApplicationUser, UserResource>(user);
          return new OkObjectResult(new CreatedResource(
            "User Created",
            userRes));
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError(error.Code, error.Description);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserResource updateUserResource, string id)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(id);
        if (user == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            "User not found"
          ));
        }

        this.mapper.Map<UpdateUserResource, ApplicationUser>(updateUserResource, user);

        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
          user = await this.userRepository.GetUserById(user.Id);
          var userResource = this.mapper.Map<ApplicationUser, UserResource>(user);

          return new OkObjectResult(new OkResource(
            "User Updated",
            userResource
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync(string id)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(id);
        if (user == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            "User not found"
          ));
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
          user = await this.userRepository.GetUserById(user.Id);
          var userResource = this.mapper.Map<ApplicationUser, UserResource>(user);

          return new OkObjectResult(new OkResource(
            "User has disactivated",
            userResource
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

    [HttpPut("{id}/Activate")]
    public async Task<IActionResult> ActivateUserAsync(string id)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(id);
        if (user == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            "User not found"
          ));
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
          user = await this.userRepository.GetUserById(user.Id);
          var userResource = this.mapper.Map<ApplicationUser, UserResource>(user);

          return new OkObjectResult(new OkResource(
            "User has activated",
            userResource
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

    [Authorize(Policy = "EditRolePolicy")]
    [HttpPost("{id}/AssignRole")]
    public async Task<IActionResult> AddUserToRoleAsync(string id, [FromQuery] string roleId)
    {
      var user = await this.userManager.FindByIdAsync(id);
      if (user == null)
      {
        return new BadRequestObjectResult(new BadRequestResource(
          "User not found"
        ));
      }

      var role = await roleManager.FindByIdAsync(roleId);
      if (role == null)
      {
        return new BadRequestObjectResult(new BadRequestResource(
          "Role not found"
        ));
      }

      if (await userManager.IsInRoleAsync(user, role.Name))
      {
        return new BadRequestObjectResult(new BadRequestResource(
          $"User is already added to role {role.Name}"
        ));
      }

      var result = await userManager.AddToRoleAsync(user, role.Name);
      if (result.Succeeded)
      {
        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
          $"User ({user.FirstName}) was added to role ({role.Name})"
        ));
      }

      foreach (IdentityError error in result.Errors)
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

    [Authorize(Policy = "EditRolePolicy")]
    [HttpDelete("{id}/AssignRole")]
    public async Task<IActionResult> RemoveUserFromRoleAsync(string id, [FromQuery] string roleId)
    {
      var user = await this.userManager.FindByIdAsync(id);
      if (user == null)
      {
        return new BadRequestObjectResult(new BadRequestResource(
          "User not found"
        ));
      }

      var role = await roleManager.FindByIdAsync(roleId);
      if (role == null)
      {
        return new BadRequestObjectResult(new BadRequestResource(
          "Role not found"
        ));
      }

      if (!await userManager.IsInRoleAsync(user, role.Name))
      {
        return new BadRequestObjectResult(new BadRequestResource(
          $"User is already not in role {role.Name}"
        ));
      }

      var result = await userManager.RemoveFromRoleAsync(user, role.Name);
      if (result.Succeeded)
      {
        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
          $"User ({user.FirstName}) was removed from role ({role.Name})"
        ));
      }

      foreach (IdentityError error in result.Errors)
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
  }
}