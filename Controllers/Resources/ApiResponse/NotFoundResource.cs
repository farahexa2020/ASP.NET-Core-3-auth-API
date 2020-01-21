using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class NotFoundResource : ApiResponseResource
  {
    public NotFoundResource()
        : base(404, HttpStatusCode.NotFound.ToString())
    {
    }

    public NotFoundResource(string message)
        : base(404, HttpStatusCode.NotFound.ToString(), message)
    {
    }

    public NotFoundResource(string message, IEnumerable<ValidationErrorResource> errors)
        : base(404, HttpStatusCode.NotFound.ToString(), message, errors)
    {
    }
  }
}