using System.Threading.Tasks;
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketPriorityRepository
  {
    Task<SupportTicketPriority> FindTicketPriorityByIdAsync(int id);

    bool IsPriorityExist(string priorityName);

    bool IsPriorityUpdatedNameExist(string PriorityName, int updatedPriorityId);

    Task<QueryResult<SupportTicketPriority>> GetTicketPrioritiesAsync();

    void CreateTicketPriority(SupportTicketPriority supportTicketPriority);

    void UpdateTicketPriority(SupportTicketPriority supportTicketPriority);

    void DeleteTicketPriority(SupportTicketPriority supportTicketPriority);
  }
}