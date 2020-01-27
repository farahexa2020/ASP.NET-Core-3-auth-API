using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.Models;

namespace WebApp1.Persistence
{
  public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
  {
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<BankTranslation> BankTranslations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Value> Values { get; set; }
    public DbSet<ValueNotification> ValueNotifications { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      {
        foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
      }

      builder.Entity<ApplicationUser>(b =>
        {
          // Each User can have many entries in the UserRole join table
          b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

      builder.Entity<ApplicationRole>(b =>
        {
          // Each Role can have many entries in the UserRole join table
          b.HasMany(e => e.UserRoles)
              .WithOne(e => e.Role)
              .HasForeignKey(ur => ur.RoleId)
              .IsRequired();
        });

      // Configure many to many
      builder.Entity<BankTranslation>().HasKey(ct => new { ct.LanguageId, ct.BankId });
    }
  }
}
