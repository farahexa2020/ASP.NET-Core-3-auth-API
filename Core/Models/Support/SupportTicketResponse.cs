using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models.Support
{
  [Table("SupportTicketResponses")]
  public class SupportTicketResponse
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    [MaxLength]
    public string Content { get; set; }

    [Required]
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    [Required]
    public int TicketId { get; set; }

    public SupportTicket Ticket { get; set; }

    public DateTime PostedAt { get; set; }
  }
}