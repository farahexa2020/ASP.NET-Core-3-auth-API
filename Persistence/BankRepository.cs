using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Extensions;

namespace WebApp1.Persistence
{
  public class BankRepository : IBankRepository
  {
    private readonly DataDbContext context;
    public BankRepository(DataDbContext context)
    {
      this.context = context;

    }

    public async Task<QueryResult<Bank>> GetBanksAsync(BankQuery queryObj, string languageId)
    {
      var result = new QueryResult<Bank>();

      var query = this.context.Banks
                      .Include(b => b.Translations)
                      .Where(b => b.IsActive)
                      .AsQueryable();

      if (string.IsNullOrWhiteSpace(queryObj.Name))
      {
        query = query.Where(b => b.Translations.Select(t => t.LanguageId).Contains(languageId));
      }

      if (queryObj.IsActive.HasValue)
      {
        query = query.Where(u => u.IsActive == queryObj.IsActive.Value);
      }

      var columnsMap = new Dictionary<string, Expression<Func<Bank, object>>>()
      {
        ["Name"] = u => u.Translations.Select(t => t.Name)
      };

      result.TotalItems = await query.CountAsync();

      query = query.ApplyOrdering(queryObj, columnsMap);

      query = query.ApplyPaging(queryObj);

      result.Items = await query.ToListAsync();

      return result;
    }

    public async Task<Bank> GetBankByIdAsync(int id)
    {
      return await this.context.Banks
                    .Include(b => b.Translations)
                    .Where(b => b.Id == id)
                    .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Bank>> GetActiveBanksAsync()
    {
      return await this.context.Banks
          .Include(b => b.Translations)
          .Where(b => b.IsActive)
          .ToListAsync();
    }

    public void Add(Bank bank)
    {
      this.context.Add(bank);
    }

    public void Remove(Bank bank)
    {
      this.context.Remove(bank);
    }

    public async Task<IEnumerable<BankTranslation>> GetBankTranslation(int bankId)
    {
      return await this.context.BankTranslations.Where(bt => bt.BankId == bankId).ToListAsync();
    }

    public async Task AddBankTranslation(int bankId, BankTranslation bankTranslation)
    {
      var bank = await this.context.Banks.Where(b => b.Id == bankId).SingleOrDefaultAsync();

      bank.Translations.Add(bankTranslation);

      this.context.Update(bank);
    }

    public async Task UpdateBankTranslation(int bankId, BankTranslation bankTranslation, string languageId)
    {
      var bank = await this.context.Banks.Where(b => b.Id == bankId)
                                         .Include(b => b.Translations)
                                         .SingleOrDefaultAsync();

      if (bank.Translations.Select(t => t.LanguageId).Contains(languageId))
      {
        context.BankTranslations.Remove(bank.Translations.Where(t => t.LanguageId == languageId).FirstOrDefault());
        bank.Translations.Remove(bank.Translations.Where(t => t.LanguageId == languageId).FirstOrDefault());
      }

      bank.Translations.Add(bankTranslation);

      this.context.Update(bank);
    }
  }
}