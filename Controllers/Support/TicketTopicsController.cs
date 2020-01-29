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
  public class TicketTopicsController : Controller
  {
    private readonly ITicketTopicRepository ticketTopicRepository;
    private readonly IMapper mapper;

    public IUnitOfWork unitOfWork { get; }

    public TicketTopicsController(IUnitOfWork unitOfWork,
                                  ITicketTopicRepository ticketTopicRepository,
                                  IMapper mapper)
    {
      this.mapper = mapper;
      this.unitOfWork = unitOfWork;
      this.ticketTopicRepository = ticketTopicRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetTicketTopicsAsync()
    {
      var supportTicketTopics = await this.ticketTopicRepository.GetTicketTopicsAsync();

      var result = this.mapper.Map<QueryResult<SupportTicketTopic>, QueryResultResource<SupportTicketTopicResource>>(supportTicketTopics);

      return new OkObjectResult(new OkResource(
          $"All ticket topics",
          result
      ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindTicketTopicById([FromRoute] string id)
    {
      var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(id);

      if (ticketTopic == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
            "Ticket topic not found"
        ));
      }

      var result = this.mapper.Map<SupportTicketTopic, SupportTicketTopicResource>(ticketTopic);

      return new OkObjectResult(new OkResource(
          $"Ticket topic with Id ({id})",
          result
      ));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketTopic([FromBody] CreateSupportTicketTopicResource createSupportTicketTopicReource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketTopicRepository.IsTopicExist(createSupportTicketTopicReource.Name))
        {
          return new BadRequestObjectResult(new BadRequestResource(
              $"Ticket topic with name ({createSupportTicketTopicReource.Name}) is already exist!"
          ));
        }

        var supportTicketTopic = this.mapper.Map<CreateSupportTicketTopicResource, SupportTicketTopic>(createSupportTicketTopicReource);
        supportTicketTopic.CreatedAt = DateTime.Now;
        supportTicketTopic.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        supportTicketTopic.CreatedBy = loggedInUserId;

        this.ticketTopicRepository.CreateTicketTopic(supportTicketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
            $"New ticket topic has created with name ({supportTicketTopic.Name})"
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
    public async Task<IActionResult> UpdateTicketTopic([FromRoute] string id, [FromBody] CreateSupportTicketTopicResource createSupportTicketTopicResource)
    {
      if (ModelState.IsValid)
      {
        var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(id);

        if (ticketTopic == null)
        {
          return new NotFoundObjectResult(new NotFoundResource(
              "Ticket topic not found"
          ));
        }

        if (this.ticketTopicRepository.IsTopicUpdatedNameExist(
            createSupportTicketTopicResource.Name,
            id))
        {
          return new BadRequestObjectResult(new BadRequestResource(
              $"Ticket topic with name ({createSupportTicketTopicResource.Name}) is already exist!"
          ));
        }

        this.mapper.Map<CreateSupportTicketTopicResource, SupportTicketTopic>(createSupportTicketTopicResource, ticketTopic);
        ticketTopic.UpdatedAt = DateTime.Now;

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketTopicRepository.UpdateTicketTopic(ticketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
            "Ticket topic has updated"
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
    public async Task<IActionResult> DeleteTicketTopic([FromRoute] string id)
    {
      var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(id);

      if (ticketTopic == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
            "Ticket topic not found"
        ));
      }

      this.ticketTopicRepository.DeleteTicketTopic(ticketTopic);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new OkResource(
          "Ticket topic has deleted"
      ));
    }
  }
}