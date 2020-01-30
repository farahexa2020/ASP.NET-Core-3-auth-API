using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public static class MySeedClass
  {
    public static void Seed(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
      SeedLanguage(context);

      SeedRoles(roleManager);
      SeedUsers(userManager, context);

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
      IList<string> roles = new List<string>();
      foreach (var role in Enum.GetValues(typeof(Roles)))
      {
        roles.Add(role.ToString());
      }

      foreach (var role in Enum.GetValues(typeof(Roles)))
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
      if (userManager.FindByEmailAsync("admin@admin.com").Result == null)
      {
        var user = new ApplicationUser
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
          userManager.AddToRoleAsync(user, Roles.Admin.ToString()).Wait();
          userManager.AddToRoleAsync(user, Roles.User.ToString()).Wait();
        }
      }
    }
  }
}