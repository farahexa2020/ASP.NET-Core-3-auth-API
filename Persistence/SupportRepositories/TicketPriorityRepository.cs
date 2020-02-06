using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

namespace WebApp1.Persistence.SupportRepositories
{
  public class TicketPriorityRepository : ITicketPriorityRepository
  {
    private readonly ApplicationDbContext context;
    public TicketPriorityRepository(ApplicationDbContext context)
    {
      this.context = context;
    }
    public async Task<SupportTicketPriority> FindTicketPriorityByIdAsync(int id)
    {
      return await this.context.SupportTicketPriorities.Where(stp => stp.Id == id).SingleOrDefaultAsync();
    }
    public bool IsPriorityExist(string priorityName)
    {
      return this.context.SupportTicketPriorities.Select(stp => stp.Name).ToList().Contains(priorityName);
    }

    public bool IsPriorityUpdatedNameExist(string priorityName, int updatedPriorityId)
    {
      return this.context.SupportTicketPriorities
                            .Where(stp => stp.Id != updatedPriorityId)
                            .Select(stp => stp.Name)
                            .ToList()
                            .Contains(priorityName);
    }

    public async Task<QueryResult<SupportTicketPriority>> GetTicketPrioritiesAsync()
    {
      var items = this.context.SupportTicketPriorities.AsQueryable();

      var result = new QueryResult<SupportTicketPriority>();
      result.TotalItems = await items.CountAsync();
      result.Items = await items.ToListAsync();

      return result;
    }

    public void CreateTicketPriority(SupportTicketPriority supportTicketPriority)
    {
      this.context.Add(supportTicketPriority);
    }

    public void UpdateTicketPriority(SupportTicketPriority supportTicketPriority)
    {
      this.context.Update(supportTicketPriority);
    }

    public void DeleteTicketPriority(SupportTicketPriority supportTicketPriorities)
    {
      this.context.Remove(supportTicketPriorities);
    }
  }
}