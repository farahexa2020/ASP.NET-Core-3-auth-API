using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources
{
  public class LoginUserResource
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}