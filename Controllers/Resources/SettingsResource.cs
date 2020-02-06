using WebApp1.Core.Models.Support;

namespace WebApp1.Controllers.Resources
{
  public class SettingsResource
  {
    public bool isSupportTicketAutoAssignment { get; set; }

    public SupportTicketAssignmentMethod SupportTicketAssignmentMetod { get; set; }
  }
}