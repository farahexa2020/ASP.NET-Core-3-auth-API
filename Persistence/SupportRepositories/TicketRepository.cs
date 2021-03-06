using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.ISupportRepositories;
using WebApp1.Core.Models.Support;
using WebApp1.Extensions;
using WebApp1.QueryModels;

namespace WebApp1.Persistence.SupportRepositories
{
  public class TicketRepository : ITicketRepository
  {
    private readonly ApplicationDbContext context;
    public TicketRepository(ApplicationDbContext context)
    {
      this.context = context;
    }

    private enum QueryFilter
    {
      UserId = 1,
      TopicId = 2,
      StatusId = 3,
      PriorityId = 4,
    }
    public async Task<QueryResult<SupportTicket>> GetAllTicketsAsync(SupportTicketQuery queryObj)
    {
      var query = this.context.SupportTickets
                                  .Include(st => st.User)
                                  .ThenInclude(stu => stu.UserRoles)
                                  .ThenInclude(stur => stur.Role)
                                  .Include(st => st.Status)
                                  .Include(st => st.Topic)
                                  .Include(st => st.Priority)
                                  .AsQueryable();

      var FilterColumnsMap = new Dictionary<string, Expression<Func<SupportTicket, bool>>>()
      {
        ["UserId"] = st => st.UserId == queryObj.UserId,
        ["TopicId"] = st => st.TopicId == queryObj.TopicId,
        ["StatusId"] = st => st.StatusId == queryObj.StatusId,
        ["PriorityId"] = st => st.PriorityId == queryObj.PriorityId
      };

      query = this.ApplyTicketFiltering(query, queryObj, FilterColumnsMap);

      query.ApplyPaging(queryObj);

      var result = new QueryResult<SupportTicket>();
      result.TotalItems = await query.CountAsync();
      result.Items = await query.ToListAsync();

      return result; ;
    }

    public async Task<SupportTicket> FindTicketByIdAsync(int id)
    {
      return await this.context.SupportTickets
                                .Where(st => st.Id == id)
                                .Include(st => st.User)
                                .ThenInclude(stu => stu.UserRoles)
                                .ThenInclude(stur => stur.Role)
                                .Include(st => st.Topic)
                                .Include(st => st.Status)
                                .Include(st => st.Priority)
                                .FirstOrDefaultAsync();
    }

    public async Task<QueryResult<SupportTicket>> GetAllUserTicketsAsync(string userId)
    {
      var userTickets = this.context.SupportTickets
                                      .Where(st => st.UserId == userId)
                                      .Include(st => st.User)
                                      .ThenInclude(stu => stu.UserRoles)
                                      .ThenInclude(stur => stur.Role)
                                      .Include(st => st.Topic)
                                      .Include(st => st.Status)
                                      .Include(st => st.Priority)
                                      .AsQueryable();

      var result = new QueryResult<SupportTicket>();
      result.TotalItems = await userTickets.CountAsync();
      result.Items = await userTickets.ToListAsync();

      return result;
    }

    public void CreateTicket(SupportTicket supportTicket)
    {
      this.context.Add(supportTicket);
    }

    public async Task<QueryResult<SupportTicketResponse>> GetAllTicketResponsesAsync(int ticketId, SupportTicketResponseQuery queryObj)
    {
      var query = this.context.SupportTicketResponses
                          .Where(str => str.TicketId == ticketId)
                          .Include(str => str.User)
                          .ThenInclude(stru => stru.UserRoles)
                          .ThenInclude(strur => strur.Role)
                          .OrderBy(str => str.PostedAt)
                          .AsQueryable();

      var result = new QueryResult<SupportTicketResponse>();
      result.TotalItems = await query.CountAsync();

      query = query.ApplyPaging(queryObj);

      result.Items = await query.ToListAsync();

      return result;
    }

    public async Task<SupportTicketResponse> FindTicketResponseByIdAsync(int id)
    {
      return await this.context.SupportTicketResponses
                                .Where(str => str.Id == id)
                                .Include(str => str.User)
                                .ThenInclude(stru => stru.UserRoles)
                                .ThenInclude(strur => strur.Role)
                                .FirstOrDefaultAsync();
    }

    public void PostTicketResponse(int ticketId, SupportTicketResponse supportTicketResponse)
    {
      this.context.SupportTicketResponses.Add(supportTicketResponse);
    }

    public void UpdateTicket(SupportTicket supportTicket)
    {
      this.context.SupportTickets.Update(supportTicket);
    }

    private IQueryable<SupportTicket> ApplyTicketFiltering(IQueryable<SupportTicket> query, SupportTicketQuery queryObj, Dictionary<string, Expression<Func<SupportTicket, bool>>> columnsMap)
    {
      if (!string.IsNullOrWhiteSpace(queryObj.UserId) && columnsMap.ContainsKey(QueryFilter.UserId.ToString()))
      {
        return query.Where(columnsMap[QueryFilter.UserId.ToString()]).AsQueryable();
      }

      if (queryObj.TopicId.HasValue && columnsMap.ContainsKey(QueryFilter.TopicId.ToString()))
      {
        return query.Where(columnsMap[QueryFilter.TopicId.ToString()]).AsQueryable();
      }

      if (queryObj.StatusId.HasValue && columnsMap.ContainsKey(QueryFilter.StatusId.ToString()))
      {
        return query.Where(columnsMap[QueryFilter.StatusId.ToString()]).AsQueryable();
      }

      if (queryObj.PriorityId.HasValue && columnsMap.ContainsKey(QueryFilter.PriorityId.ToString()))
      {
        return query.Where(columnsMap[QueryFilter.PriorityId.ToString()]).AsQueryable();
      }

      return query;
    }
  }
}