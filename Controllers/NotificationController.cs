using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Core;

namespace WebApp1.Controllers.Resources
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class NotificationsController : Controller
  {
    private readonly INotificationRepository NotificationRepository;
    private readonly IUnitOfWork unitOfWork;
    public NotificationsController(INotificationRepository NotificationRepository,
                                        IUnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
      this.NotificationRepository = NotificationRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var notifications = await this.NotificationRepository.GetNotificationsAsync(loggedInUserId);

      foreach (var notification in notifications)
      {
        notification.seen = true;
        this.NotificationRepository.UpdateNotification(notification);
      }

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(notifications);
    }

    [HttpGet("Count")]
    public async Task<IActionResult> GetUserUnseenNotificationsCount()
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var count = await this.NotificationRepository.GetUserUnseenNotificationCountAsync(loggedInUserId);
      return new OkObjectResult(new { notificationsCount = count });
    }
  }
}