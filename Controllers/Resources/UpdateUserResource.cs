using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources
{
  public class UpdateUserResource
  {
    [StringLength(255)]
    public string FirstName { get; set; }

    [StringLength(255)]
    public string LastName { get; set; }
  }
}