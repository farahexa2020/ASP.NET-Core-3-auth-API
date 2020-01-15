using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebApp1.Core.Models
{
  public class ApplicationRole : IdentityRole
  {
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    public ApplicationRole()
    {
      this.UserRoles = new List<ApplicationUserRole>();
    }
  }
}