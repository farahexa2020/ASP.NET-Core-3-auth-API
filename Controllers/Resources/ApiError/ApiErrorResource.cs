using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApp1.Controllers.Resources.ApiError
{
  public class ApiErrorResource
  {
    public int StatusCode { get; private set; }

    public string StatusDescription { get; private set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Message { get; private set; }

    public IEnumerable<ValidationErrorResource> ModelStateErrors { get; set; }

    public object Result { get; set; }

    public ApiErrorResource(int statusCode, string statusDescription)
    {
      this.StatusCode = statusCode;
      this.StatusDescription = statusDescription;
    }

    public ApiErrorResource(int statusCode, string statusDescription, string message)
        : this(statusCode, statusDescription)
    {
      this.Message = message;
    }

    public ApiErrorResource(int statusCode, string statusDescription, string message, IEnumerable<ValidationErrorResource> errors)
        : this(statusCode, statusDescription)
    {
      this.Message = message;
      this.ModelStateErrors = errors;
    }

    public ApiErrorResource(int statusCode, string statusDescription, string message, IEnumerable<ValidationErrorResource> errors, object result)
        : this(statusCode, statusDescription)
    {
      this.Message = message;
      this.ModelStateErrors = errors;
      this.Result = result;
    }
  }
}