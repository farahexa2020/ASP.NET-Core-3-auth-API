using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebApp1.Persistence;
using WebApp1.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApp1
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers().AddNewtonsoftJson();

      services.AddDbContext<DataDbContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));
      services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
          .AddEntityFrameworkStores<DataDbContext>();

      // services.AddAuthentication(options =>
      //       {
      //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      //       }).AddJwtBearer(options =>
      //       {
      //         options.TokenValidationParameters = new TokenValidationParameters
      //         {
      //           RequireExpirationTime = true,
      //           ValidateLifetime = true,
      //           ValidateIssuerSigningKey = true,
      //           ValidateIssuer = true,
      //           ValidateAudience = true,
      //           ValidIssuer = Configuration["Token:Issuer"],
      //           ValidAudience = Configuration["Token:Audience"],
      //           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"]))
      //         };
      //       });

      services.AddAutoMapper(typeof(Startup));

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddScoped<IBankRepository, BankRepository>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
      });
    }
  }
}
