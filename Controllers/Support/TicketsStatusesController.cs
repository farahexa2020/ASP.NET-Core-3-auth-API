using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiResponse;
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
  public class TicketStatusesController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    private readonly ITicketStatusRepository ticketStatusRepository;
    public TicketStatusesController(IUnitOfWork unitOfWork,
                                    ITicketStatusRepository ticketStatusRepository,
                                    IMapper mapper)
    {
      this.mapper = mapper;
      this.ticketStatusRepository = ticketStatusRepository;
      this.unitOfWork = unitOfWork;

    }

    [HttpGet]
    public async Task<IActionResult> GetTicketStatusesAsync()
    {
      var supportTicketStatuses = await this.ticketStatusRepository.GetTicketStatusesAsync();

      var result = this.mapper.Map<QueryResult<SupportTicketStatus>, QueryResultResource<SupportTicketStatusResource>>(supportTicketStatuses);

      return new OkObjectResult(new OkResource(
          $"All ticket statuses",
          result
      ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindTicketStatusById([FromRoute] string id)
    {
      var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(id);

      if (ticketStatus == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
            "Ticket status not found"
        ));
      }

      var result = this.mapper.Map<SupportTicketStatus, SupportTicketStatusResource>(ticketStatus);

      return new OkObjectResult(new OkResource(
          $"Ticket Status with Id ({id})",
          result
      ));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketStatus([FromBody] CreateSupportTicketStatusResource createSupportTicketStatusResource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketStatusRepository.IsStatusExist(createSupportTicketStatusResource.Name))
        {
          return new BadRequestObjectResult(new BadRequestResource(
              $"Ticket Status with name ({createSupportTicketStatusResource.Name}) is already exist!"
          ));
        }

        var supportTicketStatus = this.mapper.Map<CreateSupportTicketStatusResource, SupportTicketStatus>(createSupportTicketStatusResource);
        supportTicketStatus.CreatedAt = DateTime.Now;
        supportTicketStatus.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        supportTicketStatus.CreatedBy = loggedInUserId;

        this.ticketStatusRepository.CreateTicketStatus(supportTicketStatus);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
            $"New ticket status has created with name ({supportTicketStatus.Name})"
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicketStatus([FromRoute] string id, [FromBody] CreateSupportTicketStatusResource createSupportTicketStatusResource)
    {
      if (ModelState.IsValid)
      {
        var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(id);

        if (ticketStatus == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
              "Ticket status not found"
          ));
        }

        if (this.ticketStatusRepository.IsStatusUpdatedNameExist(
            createSupportTicketStatusResource.Name,
            id))
        {
          return new BadRequestObjectResult(new BadRequestResource(
              $"Ticket status with name ({createSupportTicketStatusResource.Name}) is already exist!"
          ));
        }

        this.mapper.Map<CreateSupportTicketStatusResource, SupportTicketStatus>(createSupportTicketStatusResource, ticketStatus);
        ticketStatus.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ticketStatus.CreatedBy = loggedInUserId;

        this.ticketStatusRepository.UpdateTicketStatus(ticketStatus);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
            "Ticket Status has updated"
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicketStatus([FromRoute] string id)
    {
      var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(id);

      if (ticketStatus == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
            "Ticket status not found"
        ));
      }

      this.ticketStatusRepository.DeleteTicketStatus(ticketStatus);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new OkResource(
          "Ticket status has deleted"
      ));
    }
  }
}