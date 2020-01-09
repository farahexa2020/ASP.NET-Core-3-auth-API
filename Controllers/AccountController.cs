using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
  public class AccountController : ControllerBase
  {
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly IConfiguration config;
    public AccountController(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              IConfiguration config,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IRefreshTokenRepository refreshTokenRepository)
    {
      this.config = config;
      this.unitOfWork = unitOfWork;
      this.mapper = mapper;
      this.signInManager = signInManager;
      this.refreshTokenRepository = refreshTokenRepository;
      this.userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserResource userResource)
    {
      if (!ModelState.IsValid)
      {
        return new BadRequestObjectResult(ModelState);
      }

      var user = mapper.Map<RegisterUserResource, User>(userResource);

      var result = await userManager.CreateAsync(user, userResource.Password);

      if (!result.Succeeded)
      {
        return new BadRequestObjectResult("Failed to create");
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserResource loginUserResource)
    {
      if (!ModelState.IsValid)
      {
        return new BadRequestObjectResult(ModelState);
      }

      var user = await this.userManager.FindByEmailAsync(loginUserResource.Email);

      if (user == null)
      {
        return Unauthorized();
      }

      var result = await this.signInManager.CheckPasswordSignInAsync(user, loginUserResource.Password, false);

      if (!result.Succeeded)
      {
        return Unauthorized();
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

    private async Task<string> GenerateAccessTokenAsync(User user)
    {

      // List<string> roles = (List<string>)await this.userManager.GetRolesAsync(user);

      List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

      // foreach (string role in roles)
      // {
      //   claims.Add(new Claim(ClaimTypes.Role, role));
      // }

      DateTime tokenEndTime = DateTime.Now;
      tokenEndTime = tokenEndTime.AddMonths(1);

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