using Microsoft.AspNetCore.Builder;

namespace WebApp1.Middlewares
{
  public static class LanguageHeaderExtension
  {
    public static IApplicationBuilder UseCheckLanguageHeader(
    this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<LanguageHeader>();
    }
  }
}