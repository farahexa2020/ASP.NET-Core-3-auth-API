using System.Linq;
using AutoMapper;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Core.Models;

namespace WebApp1.Mapping
{
  public class BanksProfile : Profile
  {
    public BanksProfile()
    {
      CreateMap<Bank, BankResource>()
        .ForMember(br => br.Name, opt => opt.MapFrom((b, br, nameof, context) =>
        {
          var langauge = (string)context.Items["language"];
          return b.Translations.Where(t => t.LanguageId == langauge).SingleOrDefault()?.Name;
        }));

      CreateMap<BankTranslationResource, BankTranslation>();

      CreateMap<BankTranslation, BankTranslationResource>();

      CreateMap<CreateBankResource, Bank>();
    }
  }
}