using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models
{
  [Table("Banks")]
  public class Bank
  {
    public int Id { get; set; }

    [StringLength(255)]
    public string ImageUrl { get; set; }

    [Required]
    [StringLength(255)]
    public string AccountHolderName { get; set; }

    [Required]
    [StringLength(255)]
    public string AccountNumber { get; set; }

    public int SequenceNumber { get; set; }

    public DateTime SequenceNumberUpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public ICollection<BankTranslation> Translations { get; set; }

    public Bank()
    {
      Translations = new Collection<BankTranslation>();
    }
  }
}