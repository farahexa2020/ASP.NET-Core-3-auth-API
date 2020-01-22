using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources.Bank
{
  public class CreateBankResource
  {
    [Required]
    public string KeyName { get; set; }
    public string ImageUrl { get; set; }
    [Required]
    public string AccountHolderName { get; set; }
    [Required]
    public string AccountNumber { get; set; }
    public int SequenceNumber { get; set; }
    public bool IsActive { get; set; }
    public ICollection<BankTranslationResource> Translations { get; set; }
  }
}