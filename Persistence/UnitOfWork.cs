using System.Threading.Tasks;
using WebApp1.Core;

namespace WebApp1.Persistence
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext context;
    public UnitOfWork(ApplicationDbContext context)
    {
      this.context = context;

    }

    public async Task CompleteAsync()
    {
      await this.context.SaveChangesAsync();
    }
  }
}