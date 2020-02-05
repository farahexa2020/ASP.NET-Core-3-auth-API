using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class NotificationProfile : Profile
  {
    public NotificationProfile()
    {

      CreateMap<NotificationResource, Notification>()
          .ForMember(v => v.Id, opt => opt.Ignore());

      CreateMap<Notification, NotificationResource>();
    }
  }
}