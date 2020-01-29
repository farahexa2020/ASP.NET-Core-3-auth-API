using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.ApiResponse;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core;
using WebApp1.Core.Models.Support;

namespace WebApp1.Controllers
{
  [Authorize]
  [Route("api/Support/[controller]")]
  [ApiController]
  public class TicketsController : Controller
  {
    private readonly ITicketRepository TicketRepository;
    private readonly IMapper mapper;
    public TicketsController(ITicketRepository TicketRepository,
                                    IMapper mapper)
    {
      this.mapper = mapper;
      this.TicketRepository = TicketRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
      var Ticket = await this.TicketRepository.GetAllTickets();

      var result = this.mapper.Map<IEnumerable<SupportTicket>, IEnumerable<SupportTicketResource>>(Ticket);

      return new OkObjectResult(new OkResource(
          "All Tickets",
          result
      ));
    }

    [HttpPost]
    public IActionResult CreateTicket([FromBody] SupportTicketResource supportTicketResource)
    {
      var Ticket = this.mapper.Map<SupportTicketResource, SupportTicket>(supportTicketResource);

      var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      Ticket.UserId = loggedInUserId;
      Ticket.CreatedAt = DateTime.Now;
      Ticket.UpdatedAt = DateTime.Now;

      this.TicketRepository.CreateTicket(Ticket);

      return new OkObjectResult(new OkResource(
        "New ticket has created"
      ));
    }
  }
}