using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core;
using WebApp1.Core.Models;
using WebApp1.Extensions;

namespace WebApp1.Persistence
{
  public class UserRepository : IUserRepository
  {
    public UserManager<ApplicationUser> userManager { get; set; }
    public UserRepository(UserManager<ApplicationUser> userManager)
    {
      this.userManager = userManager;

    }

    public async Task<QueryResult<ApplicationUser>> GetUsers(UserQuery queryObj)
    {
      var result = new QueryResult<ApplicationUser>();

      var query = userManager.Users.Include(u => u.UserRoles)
                                  .ThenInclude(ur => ur.Role)
                                  .AsQueryable();

      if (!string.IsNullOrWhiteSpace(queryObj.FirstName))
      {
        query = query.Where(u => u.FirstName == queryObj.FirstName);
      }
      if (!string.IsNullOrWhiteSpace(queryObj.LastName))
      {
        query = query.Where(u => u.LastName == queryObj.LastName);
      }
      if (!string.IsNullOrWhiteSpace(queryObj.Email))
      {
        query = query.Where(u => u.Email == queryObj.Email);
      }
      if (!string.IsNullOrWhiteSpace(queryObj.RoleId))
      {
        query = query.Where(u => u.UserRoles.Select(ur => ur.RoleId).Contains(queryObj.RoleId));
      }

      var columnsMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>()
      {
        ["fisrtName"] = u => u.FirstName,
        ["lastName"] = u => u.LastName,
        ["email"] = u => u.Email,
      };

      result.TotalItems = await query.CountAsync();

      query = query.ApplyOrdering(queryObj, columnsMap);

      query = query.ApplyPaging(queryObj);

      result.Items = await query.ToListAsync();

      return result;
    }
  }
}