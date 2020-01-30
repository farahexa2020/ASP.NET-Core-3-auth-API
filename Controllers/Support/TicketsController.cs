using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiResponse;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Controllers
{
  [Authorize]
  [Route("api/Support/[controller]")]
  [ApiController]
  public class TicketsController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITicketTopicRepository ticketTopicRepository;
    private readonly ITicketStatusRepository ticketStatusRepository;
    private readonly ITicketPriorityRepository ticketPriorityRepository;
    private readonly ITicketRepository TicketRepository;
    private readonly IMapper mapper;
    public TicketsController(UserManager<ApplicationUser> userManager,
                              ITicketTopicRepository ticketTopicRepository,
                              ITicketStatusRepository ticketStatusRepository,
                              ITicketPriorityRepository ticketPriorityRepository,
                              ITicketRepository TicketRepository,
                              IUnitOfWork unitOfWork,
                              IMapper mapper)
    {
      this.mapper = mapper;
      this.unitOfWork = unitOfWork;
      this.userManager = userManager;
      this.ticketTopicRepository = ticketTopicRepository;
      this.ticketStatusRepository = ticketStatusRepository;
      this.ticketPriorityRepository = ticketPriorityRepository;
      this.TicketRepository = TicketRepository;
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllTickets([FromQuery] SupportTicketQueryResource supportTicketQueryResource)
    {
      var ticketQuery = this.mapper.Map<SupportTicketQueryResource, SupportTicketQuery>(supportTicketQueryResource);
      var ticket = await this.TicketRepository.GetAllTickets(ticketQuery);

      var result = this.mapper.Map<QueryResult<SupportTicket>, QueryResultResource<SupportTicketResource>>(ticket);

      return new OkObjectResult(new OkResource(
          "All Tickets",
          result
      ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById([FromRoute] string id)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      var ticket = await this.TicketRepository.FindTicketByIdAsync(id);
      if (ticket != null)
      {
        if (await this.userManager.IsInRoleAsync(user, Roles.Admin.ToString()) ||
            loggedInUserId == ticket.UserId ||
            loggedInUserId == ticket.AssigneeId)
        {
          var result = this.mapper.Map<SupportTicket, SupportTicketResource>(ticket);

          return new OkObjectResult(new OkResource(
            $"Ticket ({id})",
            result
          ));
        }

        return new ForbidResult();
      }

      return new NotFoundObjectResult(new NotFoundResource(
        $"Ticket with Id ({id}) not found!"
      ));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllUserTickets([FromRoute] string userId)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      if (await this.userManager.IsInRoleAsync(user, Roles.Admin.ToString()) ||
          loggedInUserId == userId)
      {
        var userTickets = await this.TicketRepository.GetAllUserTickets(userId);

        var result = this.mapper.Map<QueryResult<SupportTicket>, QueryResultResource<SupportTicketResource>>(userTickets);

        return new OkObjectResult(new OkResource(
          $"User ({userId}) Tickets",
          result
        ));
      }

      return new ForbidResult();
    }

    [Authorize(Roles = "ADmin, User")]
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateSupportTicketResource createSupportTicketResource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketTopicRepository.FindTicketTopicByIdAsync(createSupportTicketResource.TopicId) == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            $"Topic with Id ({createSupportTicketResource.TopicId}) not found"
          ));
        }

        if (this.ticketStatusRepository.FindTicketStatusByIdAsync(createSupportTicketResource.StatusId) == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            $"Status with Id ({createSupportTicketResource.StatusId}) not found"
          ));
        }

        if (this.ticketPriorityRepository.FindTicketPriorityByIdAsync(createSupportTicketResource.PriorityId) == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
            $"Proirity with Id ({createSupportTicketResource.PriorityId}) not found"
          ));
        }

        var Ticket = this.mapper.Map<CreateSupportTicketResource, SupportTicket>(createSupportTicketResource);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Ticket.UserId = loggedInUserId;
        Ticket.CreatedAt = DateTime.Now;
        Ticket.UpdatedAt = DateTime.Now;
        Ticket.CreatedBy = loggedInUserId;

        this.TicketRepository.CreateTicket(Ticket);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
          "New ticket has created"
        ));
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpGet("{id}/Response")]
    public async Task<IActionResult> GetAllTicketResponses([FromRoute] string id)
    {
      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var user = await userManager.FindByIdAsync(loggedInUserId);

      var ticket = await this.TicketRepository.FindTicketByIdAsync(id);

      if (await this.userManager.IsInRoleAsync(user, Roles.Admin.ToString()) ||
            loggedInUserId == ticket.UserId ||
            loggedInUserId == ticket.AssigneeId)
      {
        var responsesQuery = await this.TicketRepository.GetAllTicketResponsesAsync(id);

        var result = this.mapper.Map<QueryResult<SupportTicketResponse>, QueryResultResource<SupportTicketResponseResource>>(responsesQuery);

        return new OkObjectResult(new OkResource(
          $"Ticket ({id}) responses",
          result
        ));
      }

      return new ForbidResult();
    }

    [HttpPost("{id}/Response")]
    public async Task<IActionResult> PostResponse([FromRoute] string id, [FromBody] CreateSupportTicketResponseResource CreateSupportTicketResponseResource)
    {
      if (ModelState.IsValid)
      {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(loggedInUserId);

        var ticket = await this.TicketRepository.FindTicketByIdAsync(id);

        if (await this.userManager.IsInRoleAsync(user, Roles.Admin.ToString()) ||
              loggedInUserId == ticket.UserId ||
              loggedInUserId == ticket.AssigneeId)
        {
          var response = this.mapper.Map<CreateSupportTicketResponseResource, SupportTicketResponse>(CreateSupportTicketResponseResource);

          response.TicketId = id;

          response.UserId = loggedInUserId;
          response.PostedAt = DateTime.Now;

          this.TicketRepository.PostTicketResponse(id, response);

          await this.unitOfWork.CompleteAsync();

          var ticketResponse = await this.TicketRepository.FindTicketResponseByIdAsync(response.Id);

          var result = this.mapper.Map<SupportTicketResponse, SupportTicketResponseResource>(ticketResponse);

          return new OkObjectResult(new OkResource(
            "Response posted",
            result
          ));
        }

        return new ForbidResult();
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }
  }
}