using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models.Support
{
  [Table("SupportTicketTopics")]
  public class SupportTicketTopic
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public ICollection<SupportTicket> Tickets { get; set; }
  }
}