using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.SupportTicket;
using WebApp1.Core.Models;
using WebApp1.Core.Models.Support;

namespace WebApp1.Mapping
{
  public class SupportTicketsProfile : Profile
  {
    public SupportTicketsProfile()
    {
      // Map Domain Models to API Resources
      CreateMap<SupportTicket, SupportTicketResource>();

      CreateMap<SupportTicketTopic, SupportTicketTopicResource>();

      CreateMap<SupportTicketStatus, SupportTicketStatusResource>();

      CreateMap<SupportTicketPriority, SupportTicketPriorityResource>();

      CreateMap<SupportTicketResponse, SupportTicketResponseResource>();

      // Map API Resources to Domain Models
      CreateMap<CreateSupportTicketResource, SupportTicket>()
        .ForMember(st => st.Id, opt => opt.Ignore())
        .ForMember(st => st.UserId, opt => opt.Ignore());

      CreateMap<CreateSupportTicketTopicResource, SupportTicketTopic>()
        .ForMember(stt => stt.Id, opt => opt.Ignore());

      CreateMap<CreateSupportTicketStatusResource, SupportTicketStatus>()
        .ForMember(sts => sts.Id, opt => opt.Ignore()); ;

      CreateMap<CreateSupportTicketPriorityResource, SupportTicketPriority>()
        .ForMember(sts => sts.Id, opt => opt.Ignore());

      CreateMap<CreateSupportTicketResponseResource, SupportTicketResponse>()
        .ForMember(sts => sts.Id, opt => opt.Ignore());

      CreateMap<SupportTicketQueryResource, SupportTicketQuery>();

      CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));
    }
  }
}