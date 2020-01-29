using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models.Support
{
  [Table("SupportTickets")]
  public class SupportTicket
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    [MaxLength]
    public string Issue { get; set; }

    [Required]
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    [Required]
    public string AssigneeId { get; set; }

    public ApplicationUser Assignee { get; set; }

    [Required]
    public string TopicId { get; set; }

    public SupportTicketTopic Topic { get; set; }

    [Required]
    public string StatusId { get; set; }

    public SupportTicketStatus Status { get; set; }

    public ICollection<SupportTicketResponse> Responses { get; set; }

    public ICollection<SupportTicketMessage> Messages { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }
  }
}