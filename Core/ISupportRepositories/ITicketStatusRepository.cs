using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketStatusRepository
  {
    Task<SupportTicketStatus> FindTicketStatusByIdAsync(string id);

    bool IsStatusExist(string statusName);

    bool IsStatusUpdatedNameExist(string statusName, string updatedStatusId);

    Task<QueryResult<SupportTicketStatus>> GetTicketStatusesAsync();

    void CreateTicketStatus(SupportTicketStatus supportTicketStatus);

    void UpdateTicketStatus(SupportTicketStatus supportTicketStatus);

    void DeleteTicketStatus(SupportTicketStatus supportTicketStatus);
  }
}