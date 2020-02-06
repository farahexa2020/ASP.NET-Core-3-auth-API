using System.Threading.Tasks;
using WebApp1.Core.Models.Support;
using WebApp1.QueryModels;

namespace WebApp1.Core.ISupportRepositories
{
  public interface ITicketTopicRepository
  {
    Task<SupportTicketTopic> FindTicketTopicByIdAsync(int id);

    bool IsTopicExist(string topicName);

    bool IsTopicUpdatedNameExist(string topicName, int updatedTopicId);

    Task<QueryResult<SupportTicketTopic>> GetTicketTopicsAsync();

    void CreateTicketTopic(SupportTicketTopic supportTicketTopic);

    void UpdateTicketTopic(SupportTicketTopic supportTicketTopic);

    void DeleteTicketTopic(SupportTicketTopic supportTicketTopic);
  }
}