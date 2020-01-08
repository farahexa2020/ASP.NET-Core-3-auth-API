using System.Threading.Tasks;

namespace WebApp1.Core.Models
{
  public interface IUnitOfWork
  {
    Task CompleteAsync();
  }
}