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

      return new OkObjectResult(result);
    }

    [HttpGet("{ticketStatusId}")]
    public async Task<IActionResult> FindTicketStatusById([FromRoute] int ticketStatusId)
    {
      var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(ticketStatusId);

      if (ticketStatus == null)
      {
        ModelState.AddModelError("", "Ticket status not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      var result = this.mapper.Map<SupportTicketStatus, SupportTicketStatusResource>(ticketStatus);

      return new OkObjectResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketStatus([FromBody] CreateSupportTicketStatusResource createSupportTicketStatusResource)
    {
      if (ModelState.IsValid)
      {
        if (this.ticketStatusRepository.IsStatusExist(createSupportTicketStatusResource.Name))
        {
          ModelState.AddModelError("", $"Ticket Status with name ({createSupportTicketStatusResource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        var supportTicketStatus = this.mapper.Map<CreateSupportTicketStatusResource, SupportTicketStatus>(createSupportTicketStatusResource);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketStatusRepository.CreateTicketStatus(supportTicketStatus);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"New ticket status has created with name ({supportTicketStatus.Name})" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPut("{ticketStatusId}")]
    public async Task<IActionResult> UpdateTicketStatus([FromRoute] int ticketStatusId, [FromBody] CreateSupportTicketStatusResource createSupportTicketStatusResource)
    {
      if (ModelState.IsValid)
      {
        var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(ticketStatusId);

        if (ticketStatus == null)
        {
          ModelState.AddModelError("", "Ticket status not found");
          return new NotFoundObjectResult(new NotFoundResource(ModelState));
        }

        if (this.ticketStatusRepository.IsStatusUpdatedNameExist(
            createSupportTicketStatusResource.Name,
            ticketStatusId))
        {
          ModelState.AddModelError("", $"Ticket status with name ({createSupportTicketStatusResource.Name}) is already exist!");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }

        this.mapper.Map<CreateSupportTicketStatusResource, SupportTicketStatus>(createSupportTicketStatusResource, ticketStatus);

        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        this.ticketStatusRepository.UpdateTicketStatus(ticketStatus);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = "Ticket Status has updated" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpDelete("{ticketStatusId}")]
    public async Task<IActionResult> DeleteTicketStatus([FromRoute] int ticketStatusId)
    {
      var ticketStatus = await this.ticketStatusRepository.FindTicketStatusByIdAsync(ticketStatusId);

      if (ticketStatus == null)
      {
        ModelState.AddModelError("", "Ticket status not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      this.ticketStatusRepository.DeleteTicketStatus(ticketStatus);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = "Ticket status has deleted" });
    }
  }
}