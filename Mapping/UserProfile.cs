using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<RegisterUserResource, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));

      CreateMap<CreateRoleResource, IdentityRole>()
                .ForMember(ir => ir.Id, opt => opt.Ignore())
                .ForMember(ir => ir.Name, opt => opt.MapFrom(rr => rr.RoleName));
    }
  }
}