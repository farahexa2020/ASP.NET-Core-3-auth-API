using Newtonsoft.Json;

namespace WebApp1.Controllers.Resources.ApiResponse
{
  public class ValidationErrorResource
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Field { get; }

    public string Message { get; }

    public ValidationErrorResource(string field, string message)
    {
      Field = field != string.Empty ? field : null;
      Message = message;
    }
  }
}