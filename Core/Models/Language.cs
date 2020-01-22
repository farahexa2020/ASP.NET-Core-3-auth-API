using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models
{
  [Table("Languages")]
  public class Language
  {
    [Key]
    public string Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }
  }
}