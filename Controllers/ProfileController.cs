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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] string id, [FromBody] UpdateUserResource updateUserResource)
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
          return new OkObjectResult(new OkResource(
            "You security code is: " + code
          ));
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      ModelState.AddModelError("", "Failed to send confirmation code");
      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                        (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpPost("{id}/VerifyPhoneNumber")]
    public async Task<ActionResult> VerifyPhoneNumber([FromRoute] string id, [FromBody] VerifyPhoneNumberResource verifyPhoneNumberResource)
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
            return new OkObjectResult(new OkResource(
            "Congratulation , the phone number is confirmed successfully"
          ));
          }
        }

        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      // If we got this far, something failed, redisplay form
      ModelState.AddModelError("", "Failed to verify phone");
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