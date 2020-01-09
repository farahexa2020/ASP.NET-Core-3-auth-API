using System.Threading.Tasks;

namespace WebApp1.Core
{
  public interface IUnitOfWork
  {
    Task CompleteAsync();
  }
}