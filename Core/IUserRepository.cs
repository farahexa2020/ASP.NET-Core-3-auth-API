using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IUserRepository
  {
    Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery queryObj);
    Task<ApplicationUser> FindUserByIdAsync(string id);
  }
}