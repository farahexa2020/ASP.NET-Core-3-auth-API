using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Persistence.SupportRepositories
{
  public class TicketStatusRepository : ITicketStatusRepository
  {
    private readonly ApplicationDbContext context;
    public TicketStatusRepository(ApplicationDbContext context)
    {
      this.context = context;
    }
    public async Task<SupportTicketStatus> FindTicketStatusByIdAsync(string id)
    {
      return await this.context.SupportTicketStatuses.Where(sts => sts.Id == id).SingleOrDefaultAsync();
    }

    public bool IsStatusExist(string statusName)
    {
      return this.context.SupportTicketStatuses.Select(sts => sts.Name).ToList().Contains(statusName);
    }

    public bool IsStatusUpdatedNameExist(string statusName, string updatedStatusId)
    {
      return this.context.SupportTicketStatuses
                            .Where(stt => stt.Id != updatedStatusId)
                            .Select(stt => stt.Name)
                            .ToList()
                            .Contains(statusName);
    }

    public async Task<QueryResult<SupportTicketStatus>> GetTicketStatusesAsync()
    {
      var items = this.context.SupportTicketStatuses.AsQueryable();

      var result = new QueryResult<SupportTicketStatus>();
      result.TotalItems = await items.CountAsync();
      result.Items = await items.ToListAsync();

      return result;
    }

    public void CreateTicketStatus(SupportTicketStatus supportTicketStatus)
    {
      this.context.Add(supportTicketStatus);
    }

    public void UpdateTicketStatus(SupportTicketStatus supportTicketStatus)
    {
      this.context.Update(supportTicketStatus);
    }

    public void DeleteTicketStatus(SupportTicketStatus supportTicketStatus)
    {
      this.context.Remove(supportTicketStatus);
    }
  }
}