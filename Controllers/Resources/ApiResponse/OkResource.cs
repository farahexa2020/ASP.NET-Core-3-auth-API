using System.Net;
using System.Collections.Generic;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class OkResource : ApiResponseResource
  {
    public OkResource()
        : base(200, HttpStatusCode.OK.ToString())
    {
    }

    public OkResource(string message)
        : base(200, HttpStatusCode.OK.ToString(), message)
    {
    }

    public OkResource(string message, object result)
        : base(200, HttpStatusCode.OK.ToString(), message, null, result)
    {
    }
  }
}