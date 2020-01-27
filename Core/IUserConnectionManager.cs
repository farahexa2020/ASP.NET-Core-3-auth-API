using System.Collections.Generic;

namespace WebApp1.Core
{
  public interface IUserConnectionManager
  {
    void KeepUserConnection(string userId, string connectionId);
    void RemoveUserConnection(string connectionId);
    List<string> GetUserConnections(string userId);
  }
}