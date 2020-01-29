using System;
using System.Collections.Generic;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SupportTicketStatusResource
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; }
  }
}