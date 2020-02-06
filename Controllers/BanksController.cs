using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Core;
using WebApp1.Core.Models;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;
using WebApp1.QueryModels;

namespace WebApp1.Controllers
{
  [Authorize(Policy = "AdminPolicy")]
  [Route("api/[controller]")]
  [ApiController]
  public class BanksController : Controller
  {
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;
    private readonly IBankRepository bankRepository;
    public BanksController(IMapper mapper, IUnitOfWork unitOfWork, IBankRepository bankRepository)
    {
      this.bankRepository = bankRepository;
      this.unitOfWork = unitOfWork;
      this.mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveBanks()
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var queryResult = await this.bankRepository.GetActiveBanksAsync();
      var queryResultResource = this.mapper.Map<QueryResult<Bank>, QueryResultResource<BankResource>>(queryResult, opt => opt.Items["language"] = language);

      return new OkObjectResult(queryResultResource);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBanks([FromQuery] BankQueryResource bankQueryResource)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var bankQuery = this.mapper.Map<BankQueryResource, BankQuery>(bankQueryResource);

      var queryResult = await this.bankRepository.GetBanksAsync(bankQuery, language);
      var queryResultResource = this.mapper.Map<QueryResult<Bank>, QueryResultResource<BankResource>>(queryResult, opt => opt.Items["language"] = language);

      return new OkObjectResult(queryResultResource);
    }

    [HttpGet("{bankId}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetBankById([FromRoute] string bankId)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var bank = await this.bankRepository.FindBankByIdAsync(bankId);
      if (bank == null)
      {
        ModelState.AddModelError("", $"Bank with Id ({bankId}) not found");
        return new NotFoundObjectResult(new NotFoundResource());
      }

      var result = this.mapper.Map<Bank, BankResource>(bank, opt => opt.Items["language"] = language);

      return new OkObjectResult(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<ActionResult> CreateBank([FromBody] CreateBankResource resource)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (ModelState.IsValid)
      {
        var bank = this.mapper.Map<CreateBankResource, Bank>(resource);
        bank.CreatedAt = DateTime.Now;
        bank.UpdatedAt = DateTime.Now;
        bank.UpdatedBy = userId;

        this.bankRepository.Add(bank);
        await this.unitOfWork.CompleteAsync();

        var createdBank = await this.bankRepository.FindBankByIdAsync(bank.Id);
        var createdBankResource = this.mapper.Map<Bank, BankResource>(createdBank, opt => opt.Items["language"] = language);

        return new OkObjectResult(createdBankResource);
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPost("{bankId}/Translations")]
    public async Task<IActionResult> AddBankTranslation([FromRoute] string bankId, [FromBody] BankTranslationResource bankTranslationResource)
    {
      if (ModelState.IsValid)
      {
        var language = Request.Headers["Accept-Language"].ToString();

        var bankTranslation = this.mapper.Map<BankTranslationResource, BankTranslation>(bankTranslationResource);

        try
        {
          await this.bankRepository.AddBankTranslation(bankId, bankTranslation);

          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new { message = $"Bank ({bankId}) updated" });
        }
        catch (DbUpdateException e)
        {
          ModelState.AddModelError("", $"Language ({language}): Name is already added ");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPut("{bankId}/Translations")]
    public async Task<IActionResult> UpdateBankTranslation([FromRoute] string bankId, [FromBody] BankTranslationResource bankTranslationResource)
    {
      if (ModelState.IsValid)
      {
        var bankTranslation = this.mapper.Map<BankTranslationResource, BankTranslation>(bankTranslationResource);

        var language = Request.Headers["Accept-Language"].ToString();
        await this.bankRepository.UpdateBankTranslationAsync(bankId, bankTranslation, language);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"Bank ({bankId}) updated" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpDelete("{bankId}/Translations")]
    public async Task<IActionResult> DeleteBankTranslation([FromRoute] string bankId, [FromQuery] string languageId)
    {
      if (ModelState.IsValid)
      {
        await this.bankRepository.DeleteBankTranslationAsync(bankId, languageId);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"Bank ({bankId}) updated, ({languageId}) name deleted" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{bankId}")]
    public async Task<ActionResult> DeleteBank([FromRoute] string bankId)
    {
      var bank = await this.bankRepository.FindBankByIdAsync(bankId);
      if (bank == null)
      {
        ModelState.AddModelError("", $"Bank with Id ({bankId}) Not Found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      bank.IsActive = false;
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = $"Bank with Id ({bankId}) has Disactivated" });
    }
  }
}