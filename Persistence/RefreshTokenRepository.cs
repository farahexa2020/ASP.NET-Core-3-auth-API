using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Data;

namespace WebApp1.Persistence
{
  public class RefreshTokenRepository : IRefreshTokenRepository
  {
    private readonly DataDbContext context;
    public RefreshTokenRepository(DataDbContext context)
    {
      this.context = context;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(RefreshToken refreshToken)
    {
      return await this.context.RefreshTokens
          .Include(rt => rt.User)
          .Where(rt => rt.Token == refreshToken.Token)
          .Where(rt => rt.AccessToken == refreshToken.AccessToken)
          .FirstOrDefaultAsync();
    }

    public void Add(RefreshToken refreshToken)
    {
      this.context.Add(refreshToken);
    }

    public void Remove(RefreshToken refreshToken)
    {
      this.context.Remove(refreshToken);
    }
  }
}