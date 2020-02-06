using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class SettingsProfile : Profile
  {
    public SettingsProfile()
    {
      CreateMap<SetSupportTicketAssignmentMethodResource, ApplicationSettings>();

      CreateMap<ApplicationSettings, SettingsResource>();
    }
  }
}