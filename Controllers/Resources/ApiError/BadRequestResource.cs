using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiError
{
  public class BadRequestResource : ApiErrorResource
  {
    public BadRequestResource()
: base(400, HttpStatusCode.BadRequest.ToString())
    {
    }

    public BadRequestResource(string message)
        : base(400, HttpStatusCode.BadRequest.ToString(), message)
    {
    }

    public BadRequestResource(string message, IEnumerable<ValidationErrorResource> errors)
        : base(400, HttpStatusCode.BadRequest.ToString(), message, errors)
    {
    }
  }
}