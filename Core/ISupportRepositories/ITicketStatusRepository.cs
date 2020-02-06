using System.Threading.Tasks;
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketStatusRepository
  {
    Task<SupportTicketStatus> FindTicketStatusByIdAsync(int id);

    bool IsStatusExist(string statusName);

    bool IsStatusUpdatedNameExist(string statusName, int updatedStatusId);

    Task<QueryResult<SupportTicketStatus>> GetTicketStatusesAsync();

    void CreateTicketStatus(SupportTicketStatus supportTicketStatus);

    void UpdateTicketStatus(SupportTicketStatus supportTicketStatus);

    void DeleteTicketStatus(SupportTicketStatus supportTicketStatus);
  }
}