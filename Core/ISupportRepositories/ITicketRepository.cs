using System.Threading.Tasks;
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketRepository
  {
    Task<QueryResult<SupportTicket>> GetAllTicketsAsync(SupportTicketQuery supportTicketQuery);

    Task<SupportTicket> FindTicketByIdAsync(int id);

    Task<QueryResult<SupportTicket>> GetAllUserTicketsAsync(string userId);

    void CreateTicket(SupportTicket supportTicket);

    Task<QueryResult<SupportTicketResponse>> GetAllTicketResponsesAsync(int ticketId, SupportTicketResponseQuery queryObj);

    Task<SupportTicketResponse> FindTicketResponseByIdAsync(int id);

    void PostTicketResponse(int ticketId, SupportTicketResponse supportTicketResponse);

    void UpdateTicket(SupportTicket supportTicket);
  }
}