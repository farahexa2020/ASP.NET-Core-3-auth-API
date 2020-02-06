using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public class SettingsRepository : ISettingsRepository
  {
    private readonly ApplicationDbContext context;
    public SettingsRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    public ApplicationSettings GetSettings()
    {
      return this.context.Settings.Include(s => s.SupportTicketAssignmentMetod).FirstOrDefault();
    }

    public void UpdateSettings(ApplicationSettings settings)
    {
      this.context.Update(settings);
    }
  }
}