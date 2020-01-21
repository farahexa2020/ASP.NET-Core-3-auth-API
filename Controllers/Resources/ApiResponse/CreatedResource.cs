using System.Collections.Generic;
using System.Net;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class CreatedResource : ApiResponseResource
  {
    public CreatedResource()
        : base(201, HttpStatusCode.OK.ToString())
    {
    }

    public CreatedResource(string message)
        : base(201, HttpStatusCode.OK.ToString(), message)
    {
    }

    public CreatedResource(string message, object result)
        : base(201, HttpStatusCode.OK.ToString(), message, null, result)
    {
    }
  }
}