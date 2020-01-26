using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebApp1.Core.Models;

namespace WebApp1.Hubs
{
  public class ValuesHub : Hub<IValuesClient>
  {
    public async Task Add(Value value)
    {
      await Clients.All.Add(value);
    }

    public async Task Delete(int id)
    {
      await Clients.All.Delete(id);
    }
  }
}