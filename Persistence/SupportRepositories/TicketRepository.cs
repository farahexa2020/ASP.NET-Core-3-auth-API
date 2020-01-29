using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models.Support;

namespace WebApp1.Persistence
{
  public class TicketRepository : ITicketRepository
  {
    private readonly ApplicationDbContext context;
    public TicketRepository(ApplicationDbContext context)
    {
      this.context = context;
    }
    public async Task<IEnumerable<SupportTicket>> GetAllTickets()
    {
      var supportTickets = await this.context.SupportTickets
                                                .Include(st => st.Status)
                                                .Include(st => st.Topic)
                                                .ToListAsync();

      return supportTickets;
    }

    public void CreateTicket(SupportTicket supportTicket)
    {
      this.context.Add(supportTicket);
    }
  }
}