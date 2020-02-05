using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public class NotificationRepository : INotificationRepository
  {
    private readonly ApplicationDbContext context;
    public NotificationRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(string userId)
    {
      return await this.context.Notifications
                                .Where(n => n.RecieverId == userId)
                                .ToListAsync();
    }

    public async Task<Notification> FindNotificationByIdAsync(string id)
    {
      var notification = await this.context.Notifications.Where(vn => vn.Id == id).FirstOrDefaultAsync();
      notification.seen = true;
      this.UpdateNotification(notification);

      return notification;
    }

    public void UpdateNotification(Notification notification)
    {
      this.context.Update(notification);
    }

    public Task<int> GetUserUnseenNotificationCountAsync(string userId)
    {
      return this.context.Notifications.Where(n => n.seen == false).CountAsync();
    }

    public void AddNotification(Notification notification)
    {
      this.context.Add(notification);
    }
  }
}