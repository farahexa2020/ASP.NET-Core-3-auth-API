using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class CreateSupportTicketResource
  {
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    [MaxLength]
    public string Issue { get; set; }

    public string UserId { get; set; }

    [Required]
    public int TopicId { get; set; }

    [Required]
    public int StatusId { get; set; }

    [Required]
    public int PriorityId { get; set; }
  }
}