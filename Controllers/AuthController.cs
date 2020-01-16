using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApp1.Controllers.Resources;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Data;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IAuthRepository authRepository;
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
                          IAuthRepository authRepository,
                          IRefreshTokenRepository refreshTokenRepository)
    {
      this.config = config;
      this.unitOfWork = unitOfWork;
      this.mapper = mapper;
      this.signInManager = signInManager;
      this.authRepository = authRepository;
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
          return new BadRequestObjectResult("Role not found");
        }

        user.UserRoles.Add(new ApplicationUserRole() { RoleId = role.Id });

        var result = await this.authRepository.CreateUserAsync(user, userResource.Password);

        if (!result.Succeeded)
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
          return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Registration error",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
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

          return new OkObjectResult(callbackUrl);
        }
      }

      return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Invalid request",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserResource loginUserResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.authRepository.GetUserByEmailAsync(loginUserResource.Email);

        if (!this.authRepository.CheckUser(user) || !await this.authRepository.CheckCredentialAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new ResponseObjectResulrResource(
            "Email and password does not match",
            null
          ));
        }
        else if (!user.EmailConfirmed && await this.authRepository.CheckCredentialAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new ResponseObjectResulrResource(
            "Email not confirmed yet",
            null
          ));
        }

        if (!await authRepository.CheckPasswordSignInAsync(user, loginUserResource.Password, false))
        {
          return new UnauthorizedObjectResult(new ResponseObjectResulrResource(
            "Email and password does not match",
            null
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

          return new OkObjectResult(response);
        }
      }

      return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Invalid request",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
    }

    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenResource resource)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var refreshToken = await this.refreshTokenRepository.GetRefreshTokenAsync(new RefreshToken { AccessToken = resource.AccessToken, Token = resource.RefreshToken });
      if (refreshToken == null) return Unauthorized();
      if (!await this.signInManager.CanSignInAsync(refreshToken.User)) return Unauthorized();

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

      return new OkObjectResult(response);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmationResource emailConfirmationResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByIdAsync(emailConfirmationResource.UserId);
        if (user == null)
        {
          return new UnauthorizedObjectResult(new ResponseObjectResulrResource(
            "Email and password does not match",
            null
          ));
        }

        var result = await this.userManager.ConfirmEmailAsync(user, emailConfirmationResource.Token);
        if (!result.Succeeded)
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }

          return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Confirmation error",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
        }
        else
        {
          return new OkObjectResult(new ResponseObjectResulrResource(
            "Congratulation , the email is confirmed successfully",
            null
          ));
        }
      }

      return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Invalid request",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
    }

    [HttpPost("resendconfirmationemail")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] LoginUserResource loginUserResource)
    {
      if (ModelState.IsValid)
      {
        var user = await this.userManager.FindByEmailAsync(loginUserResource.Email);

        if (user == null || !await this.userManager.CheckPasswordAsync(user, loginUserResource.Password))
        {
          return new UnauthorizedObjectResult(new ResponseObjectResulrResource(
            "Email and password does not match",
            null
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

          return new OkObjectResult(callbackUrl);
        }
        else
        {
          return new BadRequestObjectResult(new ResponseObjectResulrResource(
            "Email is already confirmed",
            null
          ));
        }
      }

      return new BadRequestObjectResult(
                      new ResponseObjectResulrResource("Invalid request",
                                                        ModelState.Values.Select(
                                                          e => e.Errors[0].ErrorMessage
                                                        ).ToList()));
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