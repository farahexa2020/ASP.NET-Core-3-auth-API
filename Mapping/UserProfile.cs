using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<UserResource, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
    }
  }
}