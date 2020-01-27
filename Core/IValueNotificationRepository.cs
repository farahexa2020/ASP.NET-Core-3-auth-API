using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;

namespace WebApp1.Core
{
  public interface IValueNotificationRepository
  {
    Task<IEnumerable<ValueNotification>> GetValueNotifications(string userId);

    Task<ValueNotification> GetValueNotification(string id);

    void SetValueNotificationsAsSeen(ValueNotification notifications);

    Task<int> GetNotificationCoun(string userId);

    Task<int> GetUnseenNotificationCoun(string userId);
  }
}