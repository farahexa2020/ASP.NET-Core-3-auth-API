using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core.Models
{
  public class ApplicationSettings
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public bool isSupportTicketAutoAssignment { get; set; }

    public int SupportTicketAssignmentMetodId { get; set; }

    public SupportTicketAssignmentMethod SupportTicketAssignmentMetod { get; set; }
  }
}