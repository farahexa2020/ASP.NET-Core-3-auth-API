using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IUnitOfWork unitOfWork;
    public BanksController(IBankRepository bankRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
      this.httpContextAccessor = httpContextAccessor;
      this.mapper = mapper;
      this.bankRepository = bankRepository;
    }

    [HttpGet]
    public async Task<ActionResult<BankResource>> GetActiveBanks()
    {
      var language = this.httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();

      var banks = await this.bankRepository.GetActiveBanksAsync();
      var result = this.mapper.Map<IEnumerable<Bank>, IEnumerable<BankResource>>(banks, opt => opt.Items["language"] = language);

      return Ok(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBank([FromRoute] int id)
    {
      var bank = await this.bankRepository.GetBankByIdAsync(id);
      if (bank == null)
        return new NotFoundObjectResult($"Bank with Id ({id}) Not Found");

      bank.IsActive = false;
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult($"Bank with Id ({id}) has Disactivated");
    }
  }
}