using WebApp1.Extensions;

namespace WebApp1.QueryModels
{
  public class SupportTicketResponseQuery : IQueryObject
  {
    public string SortBy { get; set; }

    public bool? IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}