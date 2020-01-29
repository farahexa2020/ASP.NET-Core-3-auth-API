using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core
{
  public interface ITicketRepository
  {
    Task<IEnumerable<SupportTicket>> GetAllTickets();

    void CreateTicket(SupportTicket supportTicket);
  }
}