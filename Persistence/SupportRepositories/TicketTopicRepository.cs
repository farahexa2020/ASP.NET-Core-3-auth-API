using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Persistence
{
  public class TicketTopicRepository : ITicketTopicRepository
  {
    private readonly ApplicationDbContext context;
    public TicketTopicRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    public async Task<SupportTicketTopic> FindTicketTopicByIdAsync(string id)
    {
      return await this.context.SupportTicketTopics.Where(stt => stt.Id == id).SingleOrDefaultAsync();
    }

    public bool IsTopicExist(string topicName)
    {
      return this.context.SupportTicketTopics.Select(stt => stt.Name).ToList().Contains(topicName);
    }

    public bool IsTopicUpdatedNameExist(string topicName, string updatedTopicId)
    {
      return this.context.SupportTicketTopics
                            .Where(stt => stt.Id != updatedTopicId)
                            .Select(stt => stt.Name)
                            .ToList()
                            .Contains(topicName);
    }

    public async Task<QueryResult<SupportTicketTopic>> GetTicketTopicsAsync()
    {
      var items = this.context.SupportTicketTopics.AsQueryable();

      var result = new QueryResult<SupportTicketTopic>();

      result.TotalItems = await items.CountAsync();
      result.Items = await items.ToListAsync();

      return result;
    }

    public void CreateTicketTopic(SupportTicketTopic supportTicketTopic)
    {

      this.context.Add(supportTicketTopic);
    }

    public void UpdateTicketTopic(SupportTicketTopic supportTicketTopic)
    {
      this.context.Update(supportTicketTopic);
    }

    public void DeleteTicketTopic(SupportTicketTopic supportTicketTopic)
    {
      this.context.Remove(supportTicketTopic);
    }
  }
}