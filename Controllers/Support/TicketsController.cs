using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;
using WebApp1.Hubs;
using WebApp1.Constants;
using WebApp1.QueryModels;

namespace WebApp1.Controllers
{
  [Authorize]
  [Route("api/Support/[controller]")]
  [ApiController]
  public class TicketsController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IUserConnectionManager userConnectionManager;
    private readonly IHubContext<NotificationHub> notificationHubContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly ITicketTopicRepository ticketTopicRepository;
    private readonly ITicketStatusRepository ticketStatusRepository;
    private readonly ITicketPriorityRepository ticketPriorityRepository;
    private readonly ITicketRepository ticketRepository;
    private readonly INotificationRepository notificationRepository;
    private readonly IMapper mapper;
    public TicketsController(IUserConnectionManager userConnectionManager,
                              IHubContext<NotificationHub> notificationHubContext,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<ApplicationRole> roleManager,
                              ITicketTopicRepository ticketTopicRepository,
                              ITicketStatusRepository ticketStatusRepository,
                              ITicketPriorityRepository ticketPriorityRepository,
                              ITicketRepository ticketRepository,
                              INotificationRepository notificationRepository,
                              IUnitOfWork unitOfWork,
                              IMapper mapper)
    {
      this.mapper = mapper;
      this.unitOfWork = unitOfWork;
      this.userConnectionManager = userConnectionManager;
      this.notificationHubContext = notificationHubContext;
      this.userManager = userManager;
      this.roleManager = roleManager;
      this.ticketTopicRepository = ticketTopicRepository;
      this.ticketStatusRepository = ticketStatusRepository;
      this.ticketPriorityRepository = ticketPriorityRepository;
      this.ticketRepository = ticketRepository;
      this.notificationRepository = notificationRepository;
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllTickets([FromQuery] SupportTicketQueryResource supportTicketQueryResource)
    {
      var ticketQuery = this.mapper.Map<SupportTicketQueryResource, SupportTicketQuery>(supportTicketQueryResource);
      var ticket = await this.ticketRepository.GetAllTicketsAsync(ticketQuery);

      var result = this.mapper.Map<QueryResult<SupportTicket>, QueryResultResource<SupportTicketResource>>(ticket);

      return new OkObjectResult(result);
    }

    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketById([FromRoute] int ticketId)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      var ticket = await this.ticketRepository.FindTicketByIdAsync(ticketId);
      if (ticket != null)
      {
        if (await this.userManager.IsInRoleAsync(user, RolesEnum.Admin.ToString()) ||
            loggedInUserId == ticket.UserId ||
            loggedInUserId == ticket.AssigneeId)
        {
          var result = this.mapper.Map<SupportTicket, SupportTicketResource>(ticket);

          return new OkObjectResult(result);
        }

        return new ForbidResult();
      }

      ModelState.AddModelError("", $"Ticket with Id ({ticketId}) not found!");
      return new NotFoundObjectResult(new NotFoundResource(ModelState));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllUserTickets([FromRoute] string userId)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      if (await this.userManager.IsInRoleAsync(user, RolesEnum.Admin.ToString()) ||
          loggedInUserId == userId)
      {
        var userTickets = await this.ticketRepository.GetAllUserTicketsAsync(userId);

        var result = this.mapper.Map<QueryResult<SupportTicket>, QueryResultResource<SupportTicketResource>>(userTickets);

        return new OkObjectResult(result);
      }

      return new ForbidResult();
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketResource createSupportTicketResource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketTopicRepository.FindTicketTopicByIdAsync(createSupportTicketResource.TopicId) == null)
        {
          ModelState.AddModelError("", $"Topic with Id ({createSupportTicketResource.TopicId}) not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        if (this.ticketStatusRepository.FindTicketStatusByIdAsync(createSupportTicketResource.StatusId) == null)
        {
          ModelState.AddModelError("", $"Status with Id ({createSupportTicketResource.StatusId}) not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        if (this.ticketPriorityRepository.FindTicketPriorityByIdAsync(createSupportTicketResource.PriorityId) == null)
        {
          ModelState.AddModelError("", $"Proirity with Id ({createSupportTicketResource.PriorityId}) not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        var Ticket = this.mapper.Map<CreateSupportTicketResource, SupportTicket>(createSupportTicketResource);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Ticket.UserId = loggedInUserId;
        Ticket.CreatedAt = DateTime.Now;
        Ticket.UpdatedAt = DateTime.Now;
        Ticket.CreatedBy = loggedInUserId;

        this.ticketRepository.CreateTicket(Ticket);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = "New ticket has created" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpGet("{ticketId}/Response")]
    public async Task<IActionResult> GetAllTicketResponses([FromRoute] int ticketId, [FromQuery] SupportTicketResponseQuery queryObj)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      var ticket = await this.ticketRepository.FindTicketByIdAsync(ticketId);

      if (await this.userManager.IsInRoleAsync(user, RolesEnum.Admin.ToString()) ||
            loggedInUserId == ticket.UserId ||
            loggedInUserId == ticket.AssigneeId)
      {
        var responsesQuery = await this.ticketRepository.GetAllTicketResponsesAsync(ticketId, queryObj);

        var result = this.mapper.Map<QueryResult<SupportTicketResponse>, QueryResultResource<SupportTicketResponseResource>>(responsesQuery);

        return new OkObjectResult(result);
      }

      return new ForbidResult();
    }

    [Authorize(Policy = "SupportTicketResponsePolicy")]
    [HttpPost("{ticketId}/Response")]
    public async Task<IActionResult> PostResponse([FromRoute] int ticketId, [FromBody] CreateSupportTicketResponseResource CreateSupportTicketResponseResource)
    {
      if (ModelState.IsValid)
      {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(loggedInUserId);

        var ticket = await this.ticketRepository.FindTicketByIdAsync(ticketId);

        var response = this.mapper.Map<CreateSupportTicketResponseResource, SupportTicketResponse>(CreateSupportTicketResponseResource);

        response.TicketId = ticketId;

        response.UserId = loggedInUserId;
        response.PostedAt = DateTime.Now;

        this.ticketRepository.PostTicketResponse(ticketId, response);

        await this.unitOfWork.CompleteAsync();

        var ticketResponse = await this.ticketRepository.FindTicketResponseByIdAsync(response.Id);

        var result = this.mapper.Map<SupportTicketResponse, SupportTicketResponseResource>(ticketResponse);

        if (loggedInUserId != ticket.UserId)
        {
          var notification = new Notification()
          {
            SenderId = loggedInUserId,
            RecieverId = ticket.UserId,
            message = $"User with id ({loggedInUserId}) added a response to ticket ({ticket.Id})",
            seen = false
          };
          this.notificationRepository.AddNotification(notification);

          await this.unitOfWork.CompleteAsync();

          try
          {
            await this.notify("sendToUser", notification.message, ticket.UserId);
          }
          catch (KeyNotFoundException e)
          {
          }
        }

        return new OkObjectResult(result);
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPost("{ticketId}/Assign")]
    public async Task<IActionResult> AssignTicket([FromRoute] int ticketId, [FromQuery] string supportId)
    {
      var ticket = await this.ticketRepository.FindTicketByIdAsync(ticketId);
      if (ticket == null)
      {
        ModelState.AddModelError("", "Ticket Not Found!");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      var user = await this.userManager.FindByIdAsync(supportId);
      if (user == null)
      {
        ModelState.AddModelError("", "User Not Found!");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      if (!await this.userManager.IsInRoleAsync(user, RolesEnum.Support.ToString()))
      {
        ModelState.AddModelError("", $"User ({user.Id}) is Not a member of role Support!");
        return new BadRequestObjectResult(new BadRequestResource(ModelState));
      }

      if (!string.IsNullOrWhiteSpace(ticket.AssigneeId))
      {
        ModelState.AddModelError("", $"Ticket ({ticket.Id}) is already assigned!");
        return new BadRequestObjectResult(new BadRequestResource(ModelState));
      }

      ticket.AssigneeId = supportId;
      this.ticketRepository.UpdateTicket(ticket);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = $"User ({supportId}) has assigned to ticket ({ticketId})" });
    }

    [HttpDelete("{ticketId}/Assign")]
    public async Task<IActionResult> UnassignTicket([FromRoute] int ticketId)
    {
      var ticket = await this.ticketRepository.FindTicketByIdAsync(ticketId);
      if (ticket == null)
      {
        ModelState.AddModelError("", "Ticket Not Found!");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      if (ticket.AssigneeId == null)
      {
        ModelState.AddModelError("", "Ticket already has no assignee!");
        return new BadRequestObjectResult(new BadRequestResource(ModelState));
      }

      ticket.AssigneeId = null;
      this.ticketRepository.UpdateTicket(ticket);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = $"Ticket ({ticketId}) has had assignee removed" });
    }

    private async Task notify(string method, object message, string userId)
    {
      var connections = this.userConnectionManager.GetUserConnections(userId);
      if (connections != null && connections.Count > 0)
      {
        foreach (var connectionId in connections)
        {
          await this.notificationHubContext.Clients.Client(connectionId).SendAsync(method, message);//send to user 
        }
      }
    }
  }
}