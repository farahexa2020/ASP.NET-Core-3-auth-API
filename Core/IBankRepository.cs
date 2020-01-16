using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IBankRepository
  {
    Task<IEnumerable<Bank>> GetBanksAsync();

    Task<Bank> GetBankByIdAsync(int id);

    Task<IEnumerable<Bank>> GetActiveBanksAsync();

    void Add(Bank bank);

    void Remove(Bank bank);
  }
}