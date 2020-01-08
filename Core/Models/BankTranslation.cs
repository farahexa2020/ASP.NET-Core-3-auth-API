using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models
{
  [Table("BankTranslations")]
  public class BankTranslation
  {
    public int Id { get; set; }

    [Required]
    public string LanguageId { get; set; }

    public Language Language { get; set; }

    [Required]
    public int BankId { get; set; }

    public Bank Bank { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }
  }
}