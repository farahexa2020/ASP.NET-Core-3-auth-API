using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IUserRepository
  {
    Task<QueryResult<ApplicationUser>> GetUsers(UserQuery queryObj);
    Task<ApplicationUser> GetUserById(string id);
  }
}