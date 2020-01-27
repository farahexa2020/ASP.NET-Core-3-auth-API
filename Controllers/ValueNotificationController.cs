using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Core;

namespace WebApp1.Controllers.Resources
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValueNotificationController : Controller
  {
    private readonly IValueNotificationRepository valueNotificationRepository;
    private readonly IUnitOfWork unitOfWork;
    public ValueNotificationController(IValueNotificationRepository valueNotificationRepository,
                                        IUnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
      this.valueNotificationRepository = valueNotificationRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetValueNotifications([FromQuery] string userId)
    {
      var notifications = await this.valueNotificationRepository.GetValueNotifications(userId);

      foreach (var notification in notifications)
      {
        this.valueNotificationRepository.SetValueNotificationsAsSeen(notification);
      }

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(notifications);
    }
  }
}