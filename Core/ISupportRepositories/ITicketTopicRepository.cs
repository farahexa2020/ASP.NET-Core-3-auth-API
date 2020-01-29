using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketTopicRepository
  {
    Task<SupportTicketTopic> FindTicketTopicByIdAsync(string id);

    bool IsTopicExist(string topicName);

    bool IsTopicUpdatedNameExist(string topicName, string updatedTopicId);

    Task<QueryResult<SupportTicketTopic>> GetTicketTopicsAsync();

    void CreateTicketTopic(SupportTicketTopic supportTicketTopic);

    void UpdateTicketTopic(SupportTicketTopic supportTicketTopic);

    void DeleteTicketTopic(SupportTicketTopic supportTicketTopic);
  }
}