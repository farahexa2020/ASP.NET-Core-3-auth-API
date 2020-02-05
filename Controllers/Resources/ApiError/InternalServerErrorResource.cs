using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiError
{
  public class InternalServerErrorResource : ApiErrorResource
  {
    public InternalServerErrorResource()
        : base(500, HttpStatusCode.InternalServerError.ToString())
    {
    }
  }
}