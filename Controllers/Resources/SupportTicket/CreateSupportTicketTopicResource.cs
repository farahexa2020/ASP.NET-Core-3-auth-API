using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class CreateSupportTicketTopicResource
  {
    public string Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }
  }
}