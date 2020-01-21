using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class InternalServerErrorResource : ApiResponseResource
  {
    public InternalServerErrorResource()
        : base(500, HttpStatusCode.InternalServerError.ToString())
    {
    }

    public InternalServerErrorResource(string message)
        : base(500, HttpStatusCode.InternalServerError.ToString(), message)
    {
    }

    public InternalServerErrorResource(string message, IEnumerable<ValidationErrorResource> errors)
        : base(500, HttpStatusCode.InternalServerError.ToString(), message, errors)
    {
    }
  }
}