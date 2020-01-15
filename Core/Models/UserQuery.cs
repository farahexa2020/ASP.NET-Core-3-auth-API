using WebApp1.Extensions;

namespace WebApp1.Core.Models
{
  public class UserQuery : IQueryObject
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string RoleId { get; set; }

    public string SortBy { get; set; }

    public bool IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}