using WebApp1.Extensions;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SupportTicketQueryResource : IQueryObject
  {
    public string UserId { get; set; }

    public string TopicId { get; set; }

    public string StatusId { get; set; }

    public string PriorityId { get; set; }

    public string SortBy { get; set; }

    public bool? IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}