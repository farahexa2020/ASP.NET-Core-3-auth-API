using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SupportTicketResource
  {
    public int Id { get; set; }

    public string Issue { get; set; }

    public UserResource User { get; set; }

    public UserResource Assignee { get; set; }

    public SupportTicketTopicResource Topic { get; set; }

    public SupportTicketStatusResource Status { get; set; }

    public SupportTicketPriorityResource Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; }
  }
}