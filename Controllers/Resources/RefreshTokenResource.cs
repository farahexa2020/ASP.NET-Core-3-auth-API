using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources
{
  public class RefreshTokenResource
  {
    [Required]
    public string AccessToken { get; set; }

    [Required]
    public string RefreshToken { get; set; }
  }
}