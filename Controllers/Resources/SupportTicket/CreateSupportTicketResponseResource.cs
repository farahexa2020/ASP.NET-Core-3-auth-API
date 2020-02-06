using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Controllers.Resources.SupportTicket
{
  public class CreateSupportTicketResponseResource
  {
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    [MaxLength]
    public string Content { get; set; }
  }
}