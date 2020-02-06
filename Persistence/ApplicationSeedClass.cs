using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApp1.Core.Claims;
using WebApp1.Core.Models;
using WebApp1.Constants;
using WebApp1.Core.Models.Support;

namespace WebApp1.Persistence
{
  public static class ApplicationSeedClass
  {
    public static void Seed(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
      SeedLanguage(context);

      SeedRoles(roleManager);
      SeedUsers(userManager, context);

      SeedSupportTicketProperties(context);

      context.SaveChanges();
    }

    public static void SeedLanguage(ApplicationDbContext context)
    {
      if (context.Languages.Count() == 0)
      {
        context.Languages.Add(
           new Language
           {
             Id = "ar",
             Name = "Arabic"
           });

        context.Languages.Add(
           new Language
           {
             Id = "en",
             Name = "English"
           });
      }
    }

    public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
    {
      foreach (var role in Enum.GetValues(typeof(RolesEnum)))
      {
        var appRole = new ApplicationRole
        {
          Name = role.ToString()
        };

        if (!roleManager.RoleExistsAsync(appRole.Name).Result)
        {
          var roleResult = roleManager.CreateAsync(appRole).Result;
        }
      }
    }

    public static void SeedUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
      var user = userManager.FindByEmailAsync("admin@admin.com").Result;
      if (user == null)
      {
        user = new ApplicationUser
        {
          UserName = "admin@admin.com",
          Email = "admin@admin.com",
          FirstName = "admin",
          LastName = "admin",
          EmailConfirmed = true,
          PhoneNumber = "000-000-0000",
          PhoneNumberConfirmed = true,
          IsActive = true
        };

        var result = userManager.CreateAsync(user, "123a123a").Result;
        if (result.Succeeded)
        {
          var roleResult = userManager.AddToRolesAsync(user, new List<string> {
                              RolesEnum.Admin.ToString(),
                              RolesEnum.User.ToString()
                            }).Result;

          if (roleResult.Succeeded)
          {
            var claimResult = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, ClaimValuesEnum.SuperAdmin.ToString())).Result;
          }
        }
      }
    }

    public static void SeedSupportTicketProperties(ApplicationDbContext context)
    {
      SeedSupportTicketTopics(context);
      SeedSupportTicketStatuses(context);
      SeedSupportTicketPriorities(context);
      SeedSupportTicketMethods(context);
    }

    public static void SeedSupportTicketTopics(ApplicationDbContext context)
    {
      foreach (var topic in Enum.GetValues(typeof(SupportTicketTopicsEnum)))
      {
        if (context.SupportTicketTopics.Where(stt => stt.Name == topic.ToString()).FirstOrDefault() == null)
        {
          var ticketTopic = new SupportTicketTopic() { Name = topic.ToString() };
          context.Add(ticketTopic);
        }
      }
    }

    public static void SeedSupportTicketStatuses(ApplicationDbContext context)
    {
      foreach (var status in Enum.GetValues(typeof(SupportTicketStatusesEnum)))
      {
        if (context.SupportTicketStatuses.Where(sts => sts.Name == status.ToString()).FirstOrDefault() == null)
        {
          var ticketStatus = new SupportTicketStatus() { Name = status.ToString() };
          context.Add(ticketStatus);
        }
      }
    }

    public static void SeedSupportTicketPriorities(ApplicationDbContext context)
    {
      foreach (var priority in Enum.GetValues(typeof(SupportTicketPrioritiesEnum)))
      {
        if (context.SupportTicketPriorities.Where(sts => sts.Name == priority.ToString()).FirstOrDefault() == null)
        {
          var ticketPriority = new SupportTicketPriority() { Name = priority.ToString() };
          context.Add(ticketPriority);
        }
      }
    }

    public static void SeedSupportTicketMethods(ApplicationDbContext context)
    {
      foreach (var method in Enum.GetValues(typeof(SupportTicketAssignmentMetodsEnum)))
      {
        if (context.SupportTicketAssignmentMethods.Where(stam => stam.Name == method.ToString()).FirstOrDefault() == null)
        {
          var ticketAssignmentMethod = new SupportTicketAssignmentMethod() { Name = method.ToString() };
          context.Add(ticketAssignmentMethod);
        }
      }
    }
  }
}