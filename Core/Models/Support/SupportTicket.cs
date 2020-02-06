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
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    [MaxLength]
    public string Issue { get; set; }

    [Required]
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    public string AssigneeId { get; set; }

    public ApplicationUser Assignee { get; set; }

    [Required]
    public int TopicId { get; set; }

    public SupportTicketTopic Topic { get; set; }

    [Required]
    public int StatusId { get; set; }

    public SupportTicketStatus Status { get; set; }

    [Required]
    public int PriorityId { get; set; }

    public SupportTicketPriority Priority { get; set; }

    public ICollection<SupportTicketResponse> Responses { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string CreatedBy { get; set; }
  }
}