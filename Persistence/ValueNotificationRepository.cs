using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public class ValueNotificationRepository
  // : IValueNotificationRepository
  {
    // private readonly ApplicationDbContext context;
    // public ValueNotificationRepository(ApplicationDbContext context)
    // {
    //   this.context = context;
    // }

    // public async Task<IEnumerable<ValueNotification>> GetValueNotifications(string userId)
    // {
    //   return await this.context.ValueNotifications.ToListAsync();
    // }

    // public async Task<ValueNotification> GetValueNotification(string id)
    // {
    //   var notification = await this.context.ValueNotifications.Where(vn => vn.Id == id).FirstOrDefaultAsync();
    //   this.SetValueNotificationsAsSeen(notification);

    //   return notification;
    // }

    // public void SetValueNotificationsAsSeen(ValueNotification notification)
    // {
    //   notification.seen = true;
    //   this.context.Update(notification);
    // }

    // public Task<int> GetNotificationCoun(string userId)
    // {
    //   return this.context.ValueNotifications.CountAsync();
    // }

    // public Task<int> GetUnseenNotificationCoun(string userId)
    // {
    //   return this.context.ValueNotifications.Where(vn => vn.seen == false).CountAsync();
    // }
  }
}