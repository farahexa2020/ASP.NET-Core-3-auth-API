using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Hubs
{
  public interface IValuesClient
  {
    Task Add(Value value);

    Task Delete(int id);
  }
}