using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class ValuesProfile : Profile
  {
    public ValuesProfile()
    {
      CreateMap<ValueResource, Value>()
          .ForMember(v => v.Id, opt => opt.Ignore());

      CreateMap<ValueNotificationResource, ValueNotification>()
          .ForMember(v => v.Id, opt => opt.Ignore());

      CreateMap<Value, ValueResource>();

      CreateMap<ValueNotification, ValueNotificationResource>();
    }
  }
}