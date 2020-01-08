using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.Models;

namespace WebApp1.Data
{
  public class DataDbContext : IdentityDbContext
  {
    public DbSet<Bank> Banks { get; set; }

    public DataDbContext(DbContextOptions<DataDbContext> options)
        : base(options)
    {
    }
  }
}
