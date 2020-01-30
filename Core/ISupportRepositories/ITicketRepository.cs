using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketRepository
  {
    Task<QueryResult<SupportTicket>> GetAllTickets(SupportTicketQuery supportTicketQuery);

    Task<SupportTicket> FindTicketByIdAsync(string id);

    Task<QueryResult<SupportTicket>> GetAllUserTickets(string userId);

    void CreateTicket(SupportTicket supportTicket);

    Task<QueryResult<SupportTicketResponse>> GetAllTicketResponsesAsync(string ticketId);

    Task<SupportTicketResponse> FindTicketResponseByIdAsync(string id);

    void PostTicketResponse(string ticketId, SupportTicketResponse supportTicketResponse);
  }
}