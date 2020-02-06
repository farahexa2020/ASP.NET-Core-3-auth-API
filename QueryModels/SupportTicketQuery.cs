using WebApp1.Extensions;

namespace WebApp1.QueryModels
{
  public class SupportTicketQuery : IQueryObject
  {
    public string UserId { get; set; }

    public int? TopicId { get; set; }

    public int? StatusId { get; set; }

    public int? PriorityId { get; set; }

    public string SortBy { get; set; }

    public bool? IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}