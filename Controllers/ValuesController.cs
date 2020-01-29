using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Hubs;
using WebApp1.Persistence;

namespace WebApp1.Controllers
{
  [Authorize(Policy = "AdminPolicy")]
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : Controller
  {
    private readonly ApplicationDbContext context;
    private readonly IHubContext<ValuesHub> hubContext;
    private readonly IMapper mapper;
    private readonly UserManager<ApplicationUser> userManager;

    public IHubContext<NotificationUserHub> notificationUserHubContext { get; }
    public IUserConnectionManager userConnectionManager { get; }

    public ValuesController(ApplicationDbContext context,
                            IHubContext<ValuesHub> hubContext,
                            IMapper mapper,
                            IHubContext<NotificationUserHub> notificationUserHubContext,
                            IUserConnectionManager userConnectionManager,
                            UserManager<ApplicationUser> userManager)
    {
      this.mapper = mapper;
      this.notificationUserHubContext = notificationUserHubContext;
      this.userConnectionManager = userConnectionManager;
      this.userManager = userManager;
      this.hubContext = hubContext;
      this.context = context;

    }
    // [HttpGet]
    // public async Task<IEnumerable<ValueResource>> GetValues()
    // {
    //   var values = await this.context.Values.ToListAsync();
    //   var result = this.mapper.Map<IEnumerable<Value>, IEnumerable<ValueResource>>(values);
    //   return result;
    // }

    // [HttpPost]
    // public async Task<IActionResult> AddValue([FromQuery] string userId, [FromBody] ValueResource valueResource)
    // {
    //   var value = this.mapper.Map<ValueResource, Value>(valueResource);

    //   this.context.Add(value);

    //   var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //   var notification = new ValueNotification()
    //   {
    //     SenderId = loggedInUserId,
    //     RecieverId = userId,
    //     message = $"User with id ({loggedInUserId}) added a value with name ({value.Name})",
    //     seen = false
    //   };
    //   this.context.Add(notification);

    //   this.context.SaveChanges();

    //   try
    //   {
    //     var connections = this.userConnectionManager.GetUserConnections(userId);
    //     if (connections != null && connections.Count > 0)
    //     {
    //       foreach (var connectionId in connections)
    //       {
    //         await notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendToUser", notification.message);//send to user 
    //       }
    //     }

    //     return new OkObjectResult("value added and message recieved");
    //   }
    //   catch (KeyNotFoundException e)
    //   {
    //     return new OkObjectResult("value added");
    //   }
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteValue(int id)
    // {
    //   var value = await this.context.Values.FindAsync(id);

    //   this.context.Remove(value);
    //   this.context.SaveChanges();

    //   await this.hubContext.Clients.All.SendAsync("Delete", value);

    //   return new OkObjectResult("Value delted");
    // }
  }
}