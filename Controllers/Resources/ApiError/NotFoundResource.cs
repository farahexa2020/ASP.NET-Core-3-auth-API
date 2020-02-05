using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp1.Controllers.Resources.ApiError
{
  public class NotFoundResource : ApiErrorResource
  {
    public NotFoundResource()
        : base(404, HttpStatusCode.NotFound.ToString())
    {
    }

    public NotFoundResource(ModelStateDictionary modelState)
        : base(404, HttpStatusCode.NotFound.ToString(), modelState)
    {
    }
  }
}