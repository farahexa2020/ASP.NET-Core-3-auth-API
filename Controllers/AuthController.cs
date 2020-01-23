using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiResponse;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IConfiguration config;
    public AuthController(IUnitOfWork unitOfWork,
                          IMapper mapper,
                          IConfiguration config,
                          UserManager<ApplicationUser> userManager,
                          RoleManager<ApplicationRole> roleManager,
                          SignInManager<ApplicationUser> signInManager,
                          IRefreshTokenRepository refreshTokenRepository)
    {
      this.config = config;
      this.unitOfWork = unitOfWork;
      this.mapper = mapper;
      this.signInManager = signInManager;
      this.refreshTokenRepository = refreshTokenRepository;
      this.userManager = userManager;
      this.roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserResource userResource)
    {
      if (ModelState.IsValid)
      {
        var user = mapper.Map<RegisterUserResource, ApplicationUser>(userResource);

        var role = await roleManager.FindByNameAsync(Roles.User.ToString());
        if (role == null)
        {
          return new BadRequestObjectResult(new BadRequestResource(
            "Role not found"
          ));
        }

        user.UserRoles.Add(new ApplicationUserRole() { RoleId = role.Id });

        user.IsActive = true;
        user.CreatedAt = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        var result = await this.userManager.CreateAsync(user, userResource.Password);

        if (!result.Succeeded)
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }

          return new BadRequestObjectResult(new BadRequestResource(
            "Invalid request",
            ModelState.Keys
            .SelectMany(key => ModelState[key].Errors.Select
                          (x => new ValidationErrorResource(key, x.ErrorMessage)))
            .ToList()
          ));
        }
        else
        {
          var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
          var callbackUrl = Url.Action(
              "ConfirmEmail",
              "Auth",
              values: new EmailConfirmationResource { UserId = user.Id, Token = token },
              protocol: Request.Scheme);

          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new CreatedResource(
            "Please confirm this accuont",
            callbackUrl
          ));
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

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserResource loginUserResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByEmailAsync(loginUserResource.Email);

        if (user == null || !await this.userManager.CheckPasswordAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Email and password does not match"
          ));
        }
        else if (!user.EmailConfirmed && await this.userManager.CheckPasswordAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Email not confirmed yet"
          ));
        }
        else if (!user.IsActive)
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "User is disactivated"
          ));
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(user, loginUserResource.Password, false);

        if (!result.Succeeded)
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Email and password does not match"
          ));
        }
        else
        {
          var newRefreshToken = new RefreshToken
          {
            UserId = user.Id,
            AccessToken = await GenerateAccessTokenAsync(user),
            Token = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now
          };

          this.refreshTokenRepository.Add(newRefreshToken);

          await this.unitOfWork.CompleteAsync();

          var response = new TokenResource()
          {
            AccessToken = newRefreshToken.AccessToken,
            RefreshToken = newRefreshToken.Token
          };

          return new OkObjectResult(new OkResource(
            "You are logged in",
            response
          ));
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

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenResource resource)
    {
      if (ModelState.IsValid)
      {
        var refreshToken = await this.refreshTokenRepository.GetRefreshTokenAsync(new RefreshToken { AccessToken = resource.AccessToken, Token = resource.RefreshToken });
        if (refreshToken == null)
        {
          return Unauthorized();
        }
        if (!await this.signInManager.CanSignInAsync(refreshToken.User))
        {
          return Unauthorized();
        }

        var newRefreshToken = new RefreshToken
        {
          UserId = refreshToken.User.Id,
          AccessToken = await GenerateAccessTokenAsync(refreshToken.User),
          Token = Guid.NewGuid().ToString(),
          CreatedAt = DateTime.Now
        };
        this.refreshTokenRepository.Add(newRefreshToken);
        await this.unitOfWork.CompleteAsync();

        var response = new TokenResource()
        {
          AccessToken = newRefreshToken.AccessToken,
          RefreshToken = newRefreshToken.Token
        };

        return new OkObjectResult(new OkResource(
          "You are logged in",
          response
        ));
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                        (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmationResource emailConfirmationResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(emailConfirmationResource.UserId);
        if (user == null)
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Email and password does not match"
          ));
        }

        var result = await this.userManager.ConfirmEmailAsync(user, emailConfirmationResource.Token);
        if (!result.Succeeded)
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }

          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Invalid request",
            ModelState.Keys
            .SelectMany(key => ModelState[key].Errors.Select
                          (x => new ValidationErrorResource(key, x.ErrorMessage)))
            .ToList()
          ));
        }
        else
        {
          return new OkObjectResult(new OkResource(
            "Congratulation , the email is confirmed successfully"
          ));
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

    [HttpPost("ResendConfirmationEmail")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] LoginUserResource loginUserResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByEmailAsync(loginUserResource.Email);

        if (user == null || !await this.userManager.CheckPasswordAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new UnAuthorizedResource(
            "Email and password does not match"
          ));
        }
        else if (!user.EmailConfirmed)
        {
          var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
          var callbackUrl = Url.Action(
              "ConfirmEmail",
              "Auth",
              values: new EmailConfirmationResource { UserId = user.Id, Token = token },
              protocol: Request.Scheme);

          return new OkObjectResult(new OkResource(
            "Please confirm this account",
            callbackUrl
          ));
        }
        else
        {
          return new BadRequestObjectResult(new BadRequestResource(
            "Email is already confirmed"
          ));
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

    [HttpGet("SendPasswordResetLink")]
    public async Task<IActionResult> SendPasswordResetLink([FromQuery] string email)
    {
      var user = await this.userManager.FindByEmailAsync(email);
      if (user == null || !(this.userManager.IsEmailConfirmedAsync(user).Result))
      {
        return new NotFoundObjectResult(new NotFoundResource(
          $"User ({email}) does not exist"
        ));
      }

      var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

      var resetLink = Url.Action("ResetPassword",
                      "Auth", new { token = token },
                       protocol: HttpContext.Request.Scheme);

      // await _emailSender.SendEmailAsync(userEmail, "Reset your password",
      //  $"Please follow this  <a href='{HtmlEncoder.Default.Encode(resetLink)}'>link </a> to reset your password.");

      return new OkObjectResult(new OkResource(
        "Please check your email",
        resetLink
      ));
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordResource resource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByEmailAsync(resource.Email);

        IdentityResult result = await this.userManager.ResetPasswordAsync(user, token, resource.Password);
        if (!result.Succeeded)
        {
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
        else
        {
          return new OkObjectResult(new OkResource(
              "reset password is done successfully"
            ));

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

    private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
      List<string> roles = (List<string>)await this.userManager.GetRolesAsync(user);

      List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

      foreach (string role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      DateTime tokenEndTime = DateTime.Now;
      tokenEndTime = tokenEndTime.AddDays(1);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["Token:Key"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      JwtSecurityToken token = new JwtSecurityToken(
          issuer: this.config["Token:Issuer"],
          audience: this.config["Token:Audience"],
          claims: claims,
          expires: tokenEndTime,
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

  }
}