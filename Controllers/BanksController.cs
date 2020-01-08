using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources;
using WebApp1.Core;
using WebApp1.Core.Models;

namespace WebApp1.Controllers
{
  [Route("api/banks")]
  public class BanksController : Controller
  {
    private readonly IBankRepository bankRepository;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;
    public BanksController(IBankRepository bankRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
      this.httpContextAccessor = httpContextAccessor;
      this.mapper = mapper;
      this.bankRepository = bankRepository;
    }

    [HttpGet]
    public async Task<ActionResult<BankResource>> GetBanks()
    {
      var language = this.httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();

      var banks = await bankRepository.GetBanks();
      var result = mapper.Map<IEnumerable<Bank>, IEnumerable<BankResource>>(banks, f => f.Items["language"] = language);

      return new OkObjectResult(result);
    }
  }
}