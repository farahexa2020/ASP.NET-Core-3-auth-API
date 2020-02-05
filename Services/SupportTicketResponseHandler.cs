using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Persistence;
using WebApp1.Persistence.SupportRepositories;

namespace WebApp1.Services
{
  public class SupportTicketResponseHandler : AuthorizationHandler<SupportTicketResponseRequirements>
  {
    public IHttpContextAccessor httpContextAccessor { get; set; }
    private readonly IServiceProvider serviceProvider;
    public SupportTicketResponseHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base()
    {
      this.serviceProvider = serviceProvider;
      this.httpContextAccessor = httpContextAccessor;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SupportTicketResponseRequirements requirement)
    {
      string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

      //   try
      //   {
      var routeValues = this.httpContextAccessor.HttpContext.Request.RouteValues;
      object editedTicketId;
      routeValues.TryGetValue("ticketId", out editedTicketId);

      using (var scope = this.serviceProvider.CreateScope())
      {
        var ticketRepositorty = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var ticket = ticketRepositorty.FindTicketByIdAsync(editedTicketId.ToString()).Result;

        if (context.User.IsInRole(Roles.Admin.ToString()) ||
              loggedInUserId == ticket.UserId ||
              loggedInUserId == ticket.AssigneeId)
        {
          context.Succeed(requirement);
        }
      }
      //   }
      //   catch (Exception e)
      //   {
      //     return Task.CompletedTask;
      //   }

      return Task.CompletedTask;
    }
  }
}