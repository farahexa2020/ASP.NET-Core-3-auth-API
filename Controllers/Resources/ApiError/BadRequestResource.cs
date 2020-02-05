using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp1.Controllers.Resources.ApiError
{
  public class BadRequestResource : ApiErrorResource
  {
    public BadRequestResource()
: base(400, HttpStatusCode.BadRequest.ToString())
    {
    }

    public BadRequestResource(ModelStateDictionary modelState)
        : base(400, HttpStatusCode.BadRequest.ToString(), modelState)
    {
    }
  }
}