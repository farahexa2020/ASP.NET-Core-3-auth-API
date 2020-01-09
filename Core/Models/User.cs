using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApp1.Core.Models
{
  public class User : IdentityUser
  {
    [Required]
    public string FirstName { get; set; }

    [Required]
    [EmailAddress]
    public string LastName { get; set; }
  }
}