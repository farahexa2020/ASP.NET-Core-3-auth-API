using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ProfileController : Controller
  {
    public IMapper mapper { get; set; }
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IUserRepository userRepository;
    public ProfileController(IMapper mapper,
                              UserManager<ApplicationUser> userManager,
                              IUserRepository userRepository)
    {
      this.userRepository = userRepository;
      this.userManager = userManager;
      this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      var user = await this.userRepository.FindUserByIdAsync(loggedInUserId);

      if (user != null)
      {
        var result = this.mapper.Map<ApplicationUser, UserResource>(user);
        return new OkObjectResult(result);
      }

      return new UnauthorizedResult();
    }

    [Authorize(Policy = "UserPolicy")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] string id, [FromBody] UpdateUserResource updateUserResource)
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
          ModelState.AddModelError("", "User Not Found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        this.mapper.Map<UpdateUserResource, ApplicationUser>(updateUserResource, user);

        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
          var updatedUser = await userRepository.FindUserByIdAsync(id);
          var updatedUserResource = this.mapper.Map<ApplicationUser, UserResource>(updatedUser);

          return new OkObjectResult(updatedUserResource);
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [Authorize(Policy = "UserPolicy")]
    [HttpPost("{id}/AddPhoneNumber")]
    // [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddPhoneNumber([FromRoute] string id, [FromQuery] string phoneNumber)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(id);
        user.PhoneNumber = phoneNumber;

        var result = await this.userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          // Generate the token 
          var code = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
          // var message = new IdentityMessage
          // {
          //   Destination = PhoneNumber,
          //   Body = "Your security code is: " + code
          // };
          return new OkObjectResult(new { message = $"You security code is: ({code})" });
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [Authorize(Policy = "UserPolicy")]
    [HttpPost("{id}/VerifyPhoneNumber")]
    public async Task<ActionResult> VerifyPhoneNumber([FromRoute] string id, [FromBody] PhoneNumberConfirmationResource verifyPhoneNumberResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(id);

        var result = await this.userManager.ChangePhoneNumberAsync(
          user,
          verifyPhoneNumberResource.PhoneNumber,
          verifyPhoneNumberResource.Code
        );
        if (result.Succeeded)
        {
          user.PhoneNumberConfirmed = true;
          var updateResult = await this.userManager.UpdateAsync(user);
          if (updateResult.Succeeded)
          {
            return new OkObjectResult(new { message = "Phone number is confirmed successfully" });
          }
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      ModelState.AddModelError("", "Failed to verify phone");
      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }
  }
}