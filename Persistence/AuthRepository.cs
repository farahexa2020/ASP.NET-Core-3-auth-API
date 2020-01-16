using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApp1.Controllers.Resources;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public class AuthRepository : IAuthRepository
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthRepository(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
    }

    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
      return await this.userManager.FindByEmailAsync(email);
    }

    public bool CheckUser(ApplicationUser user)
    {
      if (user == null)
      {
        return false;
      }
      return true;
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
      var result = await userManager.CreateAsync(user, password);

      return result;
    }

    public async Task<bool> CheckCredentialAsync(ApplicationUser user, string password)
    {
      if (!await this.userManager.CheckPasswordAsync(user, password))
      {
        return false;
      }

      return true;
    }

    public async Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
    {
      var result = await this.signInManager.CheckPasswordSignInAsync(user, password, false);

      if (!result.Succeeded)
      {
        return false;
      }

      return true;
    }
  }
}