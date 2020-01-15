using Microsoft.AspNetCore.Identity;

namespace WebApp1.Core.Models
{
  public class ApplicationUserRole : IdentityUserRole<string>
  {
    public virtual ApplicationUser User { get; set; }
    public virtual ApplicationRole Role { get; set; }
  }
}