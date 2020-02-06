using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class CreateSupportTicketPriorityResource
  {
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }
  }
}