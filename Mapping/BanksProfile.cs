using System.Linq;
using AutoMapper;
using WebApp1.Controllers.Resources;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Core.Models;
using WebApp1.QueryModels;

namespace WebApp1.Mapping
{
  public class BanksProfile : Profile
  {
    public BanksProfile()
    {
      // Map Resources to Domain Models
      CreateMap<BankTranslationResource, BankTranslation>();

      CreateMap<BankTranslation, BankTranslationResource>();

      CreateMap<CreateBankResource, Bank>();

      CreateMap<BankQueryResource, BankQuery>();

      // Map Domain Models to Resources
      CreateMap<Bank, BankResource>()
        .ForMember(br => br.Name, opt => opt.MapFrom((b, br, nameof, context) =>
        {
          var langauge = (string)context.Items["language"];
          if (b.Translations.Select(bt => bt.LanguageId).Contains(langauge))
          {
            return b.Translations.Where(t => t.LanguageId == langauge).SingleOrDefault()?.Name;
          }
          else
          {
            return b.KeyName;
          }
        }));

      CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));
    }
  }
}