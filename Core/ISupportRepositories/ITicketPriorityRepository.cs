using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketPriorityRepository
  {
    Task<SupportTicketPriority> FindTicketPriorityByIdAsync(string id);

    bool IsPriorityExist(string priorityName);

    bool IsPriorityUpdatedNameExist(string PriorityName, string updatedPriorityId);

    Task<QueryResult<SupportTicketPriority>> GetTicketPrioritiesAsync();

    void CreateTicketPriority(SupportTicketPriority supportTicketPriority);

    void UpdateTicketPriority(SupportTicketPriority supportTicketPriority);

    void DeleteTicketPriority(SupportTicketPriority supportTicketPriority);
  }
}