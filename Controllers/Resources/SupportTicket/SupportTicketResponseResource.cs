using System;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SupportTicketResponseResource
  {
    public int Id { get; set; }

    public string Content { get; set; }

    public UserResource User { get; set; }

    public DateTime PostedAt { get; set; }
  }
}