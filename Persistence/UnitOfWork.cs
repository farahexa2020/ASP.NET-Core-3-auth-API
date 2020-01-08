using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.Data;

namespace WebApp1.Persistence
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly DataDbContext context;
    public UnitOfWork(DataDbContext context)
    {
      this.context = context;

    }

    public async Task CompleteAsync()
    {
      await this.context.SaveChangesAsync();
    }
  }
}