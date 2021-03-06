using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.ApiError;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ErrorsController : Controller
  {
    [Route("{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error(int code)
    {
      HttpStatusCode parsedCode = (HttpStatusCode)code;

      var error = new ApiErrorResource(code, parsedCode.ToString());

      return new ObjectResult(error);
    }
  }
}