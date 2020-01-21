using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiResponse;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Authorize(Policy = "UserPolicy")]
  [Route("api/[controller]")]
  [ApiController]
  public class ProfileController : Controller
  {
    public IMapper mapper { get; set; }
    private readonly UserManager<ApplicationUser> userManager;
    public ProfileController(IMapper mapper, UserManager<ApplicationUser> userManager)
    {
      this.userManager = userManager;
      this.mapper = mapper;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserResource updateUserResource, [FromQuery] string id)
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            "User Not Found"
          ));
        }

        this.mapper.Map<UpdateUserResource, ApplicationUser>(updateUserResource, user);

        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
          return new OkObjectResult(new OkResource(
            "User Updated"
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
  }
}