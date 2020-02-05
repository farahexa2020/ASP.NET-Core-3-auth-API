using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Controllers.Support
{
  [Authorize]
  [Route("api/Support/[controller]")]
  [ApiController]
  public class TicketPrioritiesController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly ITicketPriorityRepository ticketPriorityRepository;
    private readonly IMapper mapper;
    public TicketPrioritiesController(IUnitOfWork unitOfWork,
                                      ITicketPriorityRepository ticketPriorityRepository,
                                      IMapper mapper)
    {
      this.mapper = mapper;
      this.ticketPriorityRepository = ticketPriorityRepository;
      this.unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetTicketTopicsAsync()
    {
      var supportTicketPriority = await this.ticketPriorityRepository.GetTicketPrioritiesAsync();

      var result = this.mapper.Map<QueryResult<SupportTicketPriority>, QueryResultResource<SupportTicketPriorityResource>>(supportTicketPriority);

      return new OkObjectResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindTicketTopicById([FromRoute] string id)
    {
      var ticketPriority = await this.ticketPriorityRepository.FindTicketPriorityByIdAsync(id);

      if (ticketPriority == null)
      {
        ModelState.AddModelError("", "Ticket topic not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      var result = this.mapper.Map<SupportTicketPriority, SupportTicketPriorityResource>(ticketPriority);

      return new OkObjectResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketTopic([FromBody] CreateSupportTicketPriorityResource createSupportTicketPriorityReource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketPriorityRepository.IsPriorityExist(createSupportTicketPriorityReource.Name))
        {
          ModelState.AddModelError("", $"Ticket priority with name ({createSupportTicketPriorityReource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        var supportTicketTopic = this.mapper.Map<CreateSupportTicketPriorityResource, SupportTicketPriority>(createSupportTicketPriorityReource);
        supportTicketTopic.CreatedAt = DateTime.Now;
        supportTicketTopic.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        supportTicketTopic.CreatedBy = loggedInUserId;

        this.ticketPriorityRepository.CreateTicketPriority(supportTicketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"New ticket priority has created with name ({supportTicketTopic.Name})" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicketTopic([FromRoute] string id, [FromBody] CreateSupportTicketPriorityResource createSupportTicketPriorityResource)
    {
      if (ModelState.IsValid)
      {
        var ticketTopic = await this.ticketPriorityRepository.FindTicketPriorityByIdAsync(id);

        if (ticketTopic == null)
        {
          ModelState.AddModelError("", "Ticket priority not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        if (this.ticketPriorityRepository.IsPriorityUpdatedNameExist(
            createSupportTicketPriorityResource.Name,
            id))
        {
          ModelState.AddModelError("", $"Ticket priority with name ({createSupportTicketPriorityResource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        this.mapper.Map<CreateSupportTicketPriorityResource, SupportTicketPriority>(createSupportTicketPriorityResource, ticketTopic);
        ticketTopic.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketPriorityRepository.UpdateTicketPriority(ticketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = "Ticket priority has updated" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicketTopic([FromRoute] string id)
    {
      var ticketTopic = await this.ticketPriorityRepository.FindTicketPriorityByIdAsync(id);

      if (ticketTopic == null)
      {
        ModelState.AddModelError("", "Ticket priority not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      this.ticketPriorityRepository.DeleteTicketPriority(ticketTopic);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = "Ticket topic has deleted" });
    }
  }
}