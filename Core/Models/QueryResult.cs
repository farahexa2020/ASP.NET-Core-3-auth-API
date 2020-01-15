using System.Collections.Generic;

namespace WebApp1.Core.Models
{
  public class QueryResult<T>
  {
    public int TotalItems { get; set; }

    public IEnumerable<T> Items { get; set; }
  }
}