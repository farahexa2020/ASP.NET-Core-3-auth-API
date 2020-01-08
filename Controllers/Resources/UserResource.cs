using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources
{
  public class UserResource
  {
    [Required]
    [StringLength(255)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(255)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string Password { get; set; }
  }
}