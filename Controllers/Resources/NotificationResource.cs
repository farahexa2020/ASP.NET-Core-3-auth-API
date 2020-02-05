namespace WebApp1.Controllers.Resources
{
  public class NotificationResource
  {
    public string Id { get; set; }

    public string SenderId { get; set; }

    public string RecieverId { get; set; }

    public string message { get; set; }

    public bool seen { get; set; }
  }
}