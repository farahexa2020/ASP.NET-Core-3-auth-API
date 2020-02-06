using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebApp1.Constants;
using WebApp1.Core;
using WebApp1.Core.ISupportRepositories;

namespace WebApp1.Services
{
  public class SupportTicketAssignmentHandler : AuthorizationHandler<SupportTicketAssignmentRequirements>
  {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IServiceProvider serviceProvider;
    public SupportTicketAssignmentHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base()
    {
      this.serviceProvider = serviceProvider;
      this.httpContextAccessor = httpContextAccessor;

    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SupportTicketAssignmentRequirements requirement)
    {
      string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
      bool isSupport = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).Contains(RolesEnum.Support.ToString());

      //   try
      //   {
      if (context.User.IsInRole(RolesEnum.Admin.ToString()))
      {
        context.Succeed(requirement);
      }
      else
      {
        var routeValues = this.httpContextAccessor.HttpContext.Request.RouteValues;
        object editedTicketId;
        routeValues.TryGetValue("ticketId", out editedTicketId);

        using (var scope = this.serviceProvider.CreateScope())
        {
          var settingsRepository = scope.ServiceProvider.GetRequiredService<ISettingsRepository>();
          var settings = settingsRepository.GetSettings();

          var ticketRepositorty = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
          var ticket = ticketRepositorty.FindTicketByIdAsync(Convert.ToInt32(editedTicketId)).Result;
          if (isSupport)
          {
            if (settings.SupportTicketAssignmentMetod.Name == SupportTicketAssignmentMetodsEnum.Responder.ToString())
            {
              if (string.IsNullOrWhiteSpace(ticket.AssigneeId) || ticket.AssigneeId == loggedInUserId)
              {
                context.Succeed(requirement);
              }
            }
          }
          else
          {
            if (loggedInUserId == ticket.UserId)
            {
              context.Succeed(requirement);
            }
          }
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