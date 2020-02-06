using System.Linq;
using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;
using WebApp1.QueryModels;

namespace WebApp1.Mapping
{
  public class UserProfile : Profile
  {
    public UserProfile()
    {
      // Map Resources to Domain Models
      CreateMap<RegisterUserResource, ApplicationUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));

      CreateMap<CreateRoleResource, ApplicationRole>()
                .ForMember(ir => ir.Id, opt => opt.Ignore())
                .ForMember(ir => ir.Name, opt => opt.MapFrom(rr => rr.RoleName));

      CreateMap<UserQueryResource, UserQuery>();

      CreateMap<UpdateUserResource, ApplicationUser>()
                .ForMember(au => au.Id, opt => opt.Ignore())
                .ForMember(au => au.Email, opt => opt.Ignore())
                .ForMember(au => au.UserName, opt => opt.Ignore());

      // Map Domain Models to Resources
      CreateMap<ApplicationUser, UserResource>()
                .ForMember(ur => ur.UserRoles,
                           opt => opt.MapFrom(u => u.UserRoles.Select(ur => new RoleResource() { Id = ur.Role.Id, Name = ur.Role.Name })));

      CreateMap<ApplicationRole, RoleResource>();

      CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));
    }
  }
}