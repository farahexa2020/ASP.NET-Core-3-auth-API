using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.Bank
{
  public class BankTranslationResource
  {
    [Required]
    public string LanguageId { get; set; }
    [Required]
    public string Name { get; set; }
  }
}