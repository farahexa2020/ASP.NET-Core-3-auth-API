using System.Collections.Generic;
using WebApp1.Extensions;

namespace WebApp1.Controllers.Resources
{
  public class UserResource
  {
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public virtual ICollection<RoleResource> UserRoles { get; set; }
  }
}