namespace WebApp1.Controllers.Resources.Bank
{
  public class BankQueryResource
  {
    public string Name { get; set; }

    public bool? IsActive { get; set; }

    public string SortBy { get; set; }

    public bool? IsSortAscending { get; set; }

    public int Page { get; set; }

    public byte PageSize { get; set; }
  }
}