using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IAuthRepository
  {
    Task<ApplicationUser> GetUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    bool CheckUser(ApplicationUser user);
    Task<bool> CheckCredentialAsync(ApplicationUser user, string password);
    Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure);
  }
}