using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface INotificationRepository
  {
    Task<IEnumerable<Notification>> GetNotificationsAsync(string userId);

    Task<Notification> FindNotificationByIdAsync(string id);

    void UpdateNotification(Notification notifications);

    Task<int> GetUserUnseenNotificationCountAsync(string userId);

    void AddNotification(Notification notification);
  }
}