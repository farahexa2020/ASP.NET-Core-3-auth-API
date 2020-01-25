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
using WebApp1.Core.Models;
using WebApp1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using WebApp1.Middlewares;

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

      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(
              Configuration.GetConnectionString("DefaultConnection")));

      // options => options.SignIn.RequireConfirmedAccount = true
      services.AddIdentity<ApplicationUser, ApplicationRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
      });

      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;

        options.SignIn.RequireConfirmedEmail = true;
      });

      services.AddAuthentication(options =>
            {
              options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
              options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = Configuration["Token:Issuer"],
                ValidAudience = Configuration["Token:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"])),
                ClockSkew = TimeSpan.Zero
              };
            });

      services.AddAuthorization(options =>
      {
        options.AddPolicy("AdminPolicy",
                          policy => policy.RequireRole(Roles.Admin.ToString()));

        options.AddPolicy("EditRolePolicy",
                          policy => policy.AddRequirements(new ManageAdminRolesRequiremnet()));

        options.AddPolicy("UserPolicy",
                          policy => policy.AddRequirements(new ManageUserRequirements()));
      });

      services.AddAutoMapper(typeof(Startup));

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddScoped<IUserRepository, UserRepository>();

      services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

      services.AddScoped<IBankRepository, BankRepository>();

      services.AddSingleton<IAuthorizationHandler, ManageAdminRolesHandler>();

      services.AddSingleton<IAuthorizationHandler, ManageUserHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // if (env.IsDevelopment())
      // {
      //   app.UseDeveloperExceptionPage();
      //   app.UseDatabaseErrorPage();
      // }
      // else
      // {
      //   app.UseExceptionHandler("/Error");
      //   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      //   app.UseHsts();
      // }

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });

      app.UseExceptionHandler("/api/Errors/500");
      app.UseStatusCodePagesWithReExecute("/api/Errors/{0}");


      app.UseCheckLanguageHeader();

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
