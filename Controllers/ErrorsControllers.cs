using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.ApiResponse;

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

      var error = new object();
      if (code == 404)
      {
        error = new ApiResponseResource(code,
                                        parsedCode.ToString(),
                                        "Requested API endpoint not found");
      }
      else if (code == 405)
      {
        error = new ApiResponseResource(code,
                                        parsedCode.ToString(),
                                        "Requested API endpoint is not allowed with the specified method");
      }
      else if (code >= 500)
      {
        error = new ApiResponseResource(code,
                                        parsedCode.ToString(),
                                        "Something went wrong, Please try again later");
      }
      else
      {
        error = new ApiResponseResource(code, parsedCode.ToString());
      }

      return new ObjectResult(error);
    }
  }
}