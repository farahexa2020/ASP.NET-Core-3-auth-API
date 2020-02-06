using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.QueryModels;

namespace WebApp1.Core
{
  public interface IUserRepository
  {
    Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery queryObj);
    Task<ApplicationUser> FindUserByIdAsync(string id);
  }
}