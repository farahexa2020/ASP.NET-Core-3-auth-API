using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Data;

namespace WebApp1.Persistence
{
  public class BankRepository : IBankRepository
  {
    private readonly DataDbContext context;
    public BankRepository(DataDbContext context)
    {
      this.context = context;

    }

    public async Task<IEnumerable<Bank>> GetBanks()
    {
      return await this.context.Banks
                        .Include(b => b.Translations)
                        .Where(b => b.IsActive)
                        .ToListAsync();
    }
  }
}