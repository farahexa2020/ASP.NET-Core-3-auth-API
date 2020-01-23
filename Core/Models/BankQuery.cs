using WebApp1.Extensions;

namespace WebApp1.Core.Models
{
  public class BankQuery : IQueryObject
  {
    public string Name { get; set; }

    public bool? IsActive { get; set; }

    public string SortBy { get; set; }

    public bool? IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}