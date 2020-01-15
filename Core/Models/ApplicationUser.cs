using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WebApp1.Extensions;

namespace WebApp1.Core.Models
{
  public class ApplicationUser : IdentityUser
  {
    [Required]
    public string FirstName { get; set; }

    [Required]
    [EmailAddress]
    public string LastName { get; set; }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    public ApplicationUser()
    {
      this.UserRoles = new List<ApplicationUserRole>();
    }
  }
}