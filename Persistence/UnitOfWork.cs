using System.Threading.Tasks;
using WebApp1.Core;

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