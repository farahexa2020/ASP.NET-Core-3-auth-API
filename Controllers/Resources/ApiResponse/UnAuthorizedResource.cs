using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class UnAuthorizedResource : ApiResponseResource
  {
    public UnAuthorizedResource()
          : base(401, HttpStatusCode.Unauthorized.ToString())
    {
    }

    public UnAuthorizedResource(string message)
          : base(401, HttpStatusCode.Unauthorized.ToString(), message)
    {
    }

    public UnAuthorizedResource(string message, IEnumerable<ValidationErrorResource> errors)
        : base(401, HttpStatusCode.Unauthorized.ToString(), message, errors)
    {
    }
  }
}