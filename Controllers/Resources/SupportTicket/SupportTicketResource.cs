using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class SupportTicketResource
  {
    public string Id { get; set; }

    [Required]
    public string Issue { get; set; }

    public string UserId { get; set; }

    [Required]
    public string TypeId { get; set; }

    [Required]
    public string StatusId { get; set; }
  }
}