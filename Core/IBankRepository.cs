using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IBankRepository
  {
    Task<QueryResult<Bank>> GetBanksAsync(BankQuery queryObj, string languageId);

    Task<Bank> FindBankByIdAsync(string id);

    Task<QueryResult<Bank>> GetActiveBanksAsync();

    void Add(Bank bank);

    void Remove(Bank bank);

    Task<IEnumerable<BankTranslation>> GetBankTranslationsAsync(string bankId);

    Task AddBankTranslation(string bankId, BankTranslation bankTranslation);

    Task UpdateBankTranslationAsync(string bankId, BankTranslation bankTranslation, string languageId);

    Task DeleteBankTranslationAsync(string bankId, string languageId);
  }
}