using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IBankRepository
  {
    Task<IEnumerable<Bank>> GetBanks();
  }
}