using System;

namespace WebApp1.Controllers.Resources.Bank
{
  public class BankResource
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public string ImageUrl { get; set; }

    public string AccountHolderName { get; set; }

    public string AccountNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
  }
}