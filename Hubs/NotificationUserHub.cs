using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebApp1.Core;

namespace WebApp1.Hubs
{
  public class NotificationUserHub : Hub
  {
    private readonly IUserConnectionManager userConnectionManager;
    public NotificationUserHub(IUserConnectionManager userConnectionManager)
    {
      this.userConnectionManager = userConnectionManager;
    }
    public string GetConnectionId()
    {
      var httpContext = this.Context.GetHttpContext();
      var userId = httpContext.Request.Query["userId"];
      userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);

      return Context.ConnectionId;
    }

    public async override Task OnDisconnectedAsync(Exception exception)
    {
      //get the connectionId
      var connectionId = Context.ConnectionId;
      userConnectionManager.RemoveUserConnection(connectionId);
      var value = await Task.FromResult(0);//adding dump code to follow the template of Hub > OnDisconnectedAsync
    }
  }
}