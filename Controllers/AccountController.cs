using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;
using WebApp1.Data;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly DataDbContext context;
    private readonly IMapper mapper;
    public AccountController(DataDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
      this.mapper = mapper;
      this.context = context;
      this.signInManager = signInManager;
      this.userManager = userManager;
    }

    public async Task<IActionResult> Register([FromBody] UserResource userResource)
    {
      if (!ModelState.IsValid)
      {
        return new BadRequestObjectResult(ModelState);
      }

      var user = mapper.Map<UserResource, User>(userResource);

      var result = await userManager.CreateAsync(user, userResource.Password);
      //   await context.SaveChangesAsync();

      if (!result.Succeeded)
      {
        return new BadRequestObjectResult("Failed to create");
      }
      else
      {
        return new OkObjectResult(user);
      }
    }
  }
}