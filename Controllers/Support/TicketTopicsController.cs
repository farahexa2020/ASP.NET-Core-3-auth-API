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
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

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

      return new OkObjectResult(result);
    }

    [HttpGet("{ticketTopicId}")]
    public async Task<IActionResult> FindTicketTopicById([FromRoute] int ticketTopicId)
    {
      var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(ticketTopicId);

      if (ticketTopic == null)
      {
        ModelState.AddModelError("", "Ticket topic not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      var result = this.mapper.Map<SupportTicketTopic, SupportTicketTopicResource>(ticketTopic);

      return new OkObjectResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketTopic([FromBody] CreateSupportTicketTopicResource createSupportTicketTopicReource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketTopicRepository.IsTopicExist(createSupportTicketTopicReource.Name))
        {
          ModelState.AddModelError("", $"Ticket topic with name ({createSupportTicketTopicReource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        var supportTicketTopic = this.mapper.Map<CreateSupportTicketTopicResource, SupportTicketTopic>(createSupportTicketTopicReource);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketTopicRepository.CreateTicketTopic(supportTicketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"New ticket topic has created with name ({supportTicketTopic.Name})" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPut("{ticketTopicId}")]
    public async Task<IActionResult> UpdateTicketTopic([FromRoute] int ticketTopicId, [FromBody] CreateSupportTicketTopicResource createSupportTicketTopicResource)
    {
      if (ModelState.IsValid)
      {
        var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(ticketTopicId);

        if (ticketTopic == null)
        {
          ModelState.AddModelError("", "Ticket topic not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        if (this.ticketTopicRepository.IsTopicUpdatedNameExist(
            createSupportTicketTopicResource.Name,
            ticketTopicId))
        {
          ModelState.AddModelError("", $"Ticket topic with name ({createSupportTicketTopicResource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        this.mapper.Map<CreateSupportTicketTopicResource, SupportTicketTopic>(createSupportTicketTopicResource, ticketTopic);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketTopicRepository.UpdateTicketTopic(ticketTopic);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = "Ticket topic has updated" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpDelete("{ticketTopicId}")]
    public async Task<IActionResult> DeleteTicketTopic([FromRoute] int ticketTopicId)
    {
      var ticketTopic = await this.ticketTopicRepository.FindTicketTopicByIdAsync(ticketTopicId);

      if (ticketTopic == null)
      {
        ModelState.AddModelError("", "Ticket topic not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      this.ticketTopicRepository.DeleteTicketTopic(ticketTopic);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = "Ticket topic has deleted" });
    }
  }
}