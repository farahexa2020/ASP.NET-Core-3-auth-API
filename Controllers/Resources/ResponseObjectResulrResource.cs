using System.Collections.Generic;

namespace WebApp1.Controllers.Resources
{
  public class ResponseObjectResulrResource
  {
    public string Message { get; set; }

    public ICollection<string> Errors { get; set; }

    public ResponseObjectResulrResource()
    {
      this.Errors = new List<string>();
    }

    public ResponseObjectResulrResource(string message, ICollection<string> errors)
    {
      this.Message = message;
      this.Errors = errors;
    }
  }
}