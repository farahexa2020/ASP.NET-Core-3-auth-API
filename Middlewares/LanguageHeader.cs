using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApp1.Middlewares
{
  public class LanguageHeader
  {
    private readonly RequestDelegate _next;

    public LanguageHeader(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      var languages = new string[] { "en", "ar" };

      if (!context.Request.Headers.Keys.Contains("Accept-Language") || !languages.Any(l => l == context.Request.Headers["Accept-Language"].ToString().ToLower()))
        context.Request.Headers["Accept-Language"] = languages.First();

      await _next.Invoke(context);
    }
  }
}