using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models
{
  [Table("Notifications")]
  public class Notification
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string SenderId { get; set; }

    public string RecieverId { get; set; }

    public string message { get; set; }

    public bool seen { get; set; }
  }
}