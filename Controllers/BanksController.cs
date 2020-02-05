using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Controllers.Resources.ApiError;
using WebApp1.Core;
using WebApp1.Core.Models;
using System.Security.Claims;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;

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

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetBankById(string id)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var bank = await this.bankRepository.FindBankByIdAsync(id);
      if (bank == null)
      {
        ModelState.AddModelError("", $"Bank with Id ({id}) not found");
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

    [HttpPost("{id}/Translations")]
    public async Task<IActionResult> AddBankTranslation([FromRoute] string id, [FromBody] BankTranslationResource bankTranslationResource)
    {
      if (ModelState.IsValid)
      {
        var language = Request.Headers["Accept-Language"].ToString();

        var bankTranslation = this.mapper.Map<BankTranslationResource, BankTranslation>(bankTranslationResource);

        try
        {
          await this.bankRepository.AddBankTranslation(id, bankTranslation);

          await this.unitOfWork.CompleteAsync();

          return new OkObjectResult(new { message = $"Bank ({id}) updated" });
        }
        catch (DbUpdateException e)
        {
          ModelState.AddModelError("", $"Language ({language}): Name is already added ");
          return new BadRequestObjectResult(new BadRequestResource(ModelState));
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpPut("{id}/Translations")]
    public async Task<IActionResult> UpdateBankTranslation([FromRoute] string id, [FromBody] BankTranslationResource bankTranslationResource)
    {
      if (ModelState.IsValid)
      {
        var bankTranslation = this.mapper.Map<BankTranslationResource, BankTranslation>(bankTranslationResource);

        var language = Request.Headers["Accept-Language"].ToString();
        await this.bankRepository.UpdateBankTranslationAsync(id, bankTranslation, language);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"Bank ({id}) updated" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [HttpDelete("{id}/Translations")]
    public async Task<IActionResult> DeleteBankTranslation([FromRoute] string id, [FromQuery] string languageId)
    {
      if (ModelState.IsValid)
      {
        await this.bankRepository.DeleteBankTranslationAsync(id, languageId);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new { message = $"Bank ({id}) updated, ({languageId}) name deleted" });
      }

      return new BadRequestObjectResult(new BadRequestResource(ModelState));
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBank([FromRoute] string id)
    {
      var bank = await this.bankRepository.FindBankByIdAsync(id);
      if (bank == null)
      {
        ModelState.AddModelError("", $"Bank with Id ({id}) Not Found");
        return new NotFoundObjectResult(new NotFoundResource(ModelState));
      }

      bank.IsActive = false;
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new { message = $"Bank with Id ({id}) has Disactivated" });
    }
  }
}