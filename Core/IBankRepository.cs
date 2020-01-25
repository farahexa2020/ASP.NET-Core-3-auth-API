using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IBankRepository
  {
    Task<QueryResult<Bank>> GetBanksAsync(BankQuery queryObj, string languageId);

    Task<Bank> GetBankByIdAsync(string id);

    Task<QueryResult<Bank>> GetActiveBanksAsync();

    void Add(Bank bank);

    void Remove(Bank bank);

    Task<IEnumerable<BankTranslation>> GetBankTranslation(string bankId);

    Task AddBankTranslation(string bankId, BankTranslation bankTranslation);

    Task UpdateBankTranslation(string bankId, BankTranslation bankTranslation, string languageId);

    Task DeleteBankTranslation(string bankId, string languageId);
  }
}