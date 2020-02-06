using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface ISettingsRepository
  {
    ApplicationSettings GetSettings();
    void UpdateSettings(ApplicationSettings settings);
  }
}