using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IRefreshTokenRepository
  {
    Task<RefreshToken> GetRefreshTokenAsync(RefreshToken refreshToken);
    void Add(RefreshToken refreshToken);
    void Remove(RefreshToken refreshToken);
  }
}