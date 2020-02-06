using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Authorize(Policy = "AdminPolicy")]
  [Route("api/[controller]")]
  [ApiController]
  public class SettingsController : Controller
  {
    private readonly ISettingsRepository settingsRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;
    public SettingsController(ISettingsRepository settingsRepository,
                                IUnitOfWork unitOfWork,
                                IMapper mapper)
    {
      this.mapper = mapper;
      this.unitOfWork = unitOfWork;
      this.settingsRepository = settingsRepository;
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
      var settings = this.settingsRepository.GetSettings();

      if (settings == null)
      {
        ModelState.AddModelError("", "Settings not found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      var result = this.mapper.Map<ApplicationSettings, SettingsResource>(settings);

      return new OkObjectResult(result);
    }


    [HttpPut("TicketAssignmetMethod")]
    public async Task<IActionResult> SetTicketAssignmentMethod([FromBody] SetSupportTicketAssignmentMethodResource methodResource)
    {
      if (methodResource.isSupportTicketAutoAssignment)
      {
        if (!methodResource.SupportTicketAssignmentMetodId.HasValue ||
            methodResource.SupportTicketAssignmentMetodId.Value == 0)
        {
          ModelState.AddModelError("", "Method type is required when Auto Assignment is enabled");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }
      }

      var settings = this.settingsRepository.GetSettings();
      if (settings == null)
      {
        settings = new ApplicationSettings();
      }
      this.mapper.Map<SetSupportTicketAssignmentMethodResource, ApplicationSettings>(methodResource, settings);

      this.settingsRepository.UpdateSettings(settings);

      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = "Support Ticket Assignment Metod has updated" });
    }
  }
}