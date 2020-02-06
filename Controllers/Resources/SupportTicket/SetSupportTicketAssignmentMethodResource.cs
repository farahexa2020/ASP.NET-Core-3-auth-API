using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SetSupportTicketAssignmentMethodResource
  {
    [Required]
    public bool isSupportTicketAutoAssignment { get; set; }

    public int? SupportTicketAssignmentMetodId { get; set; }
  }
}