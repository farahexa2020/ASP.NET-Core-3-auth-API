using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Controllers.Resources.Bank;
using WebApp1.Controllers.Resources.ApiResponse;
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

      return new OkObjectResult(new OkResource(
        "All Active Banks",
        queryResultResource
      ));
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBanks([FromQuery] BankQueryResource bankQueryResource)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var bankQuery = this.mapper.Map<BankQueryResource, BankQuery>(bankQueryResource);

      var queryResult = await this.bankRepository.GetBanksAsync(bankQuery, language);
      var queryResultResource = this.mapper.Map<QueryResult<Bank>, QueryResultResource<BankResource>>(queryResult, opt => opt.Items["language"] = language);

      return new OkObjectResult(new OkResource(
        "All Banks",
        queryResultResource
      ));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetBankById(string id)
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var bank = await this.bankRepository.GetBankByIdAsync(id);
      if (bank == null)
        return new NotFoundObjectResult(new NotFoundResource($"Bank with Id ({id}) not found"));

      var result = this.mapper.Map<Bank, BankResource>(bank, opt => opt.Items["language"] = language);

      return new OkObjectResult(new OkResource(
        $"Bank ({id})",
        result
      ));
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<ActionResult> CreateBank([FromBody] CreateBankResource resource)
    {
      string userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (!ModelState.IsValid) return BadRequest(ModelState);

      Bank bank = this.mapper.Map<CreateBankResource, Bank>(resource);
      bank.CreatedAt = DateTime.Now;
      bank.UpdatedAt = DateTime.Now;
      bank.UpdatedBy = userId;

      this.bankRepository.Add(bank);
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new CreatedResource(
        "Bank created"
      ));
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

          return new OkObjectResult(new OkResource(
            $"Bank ({id}) updated"
          ));
        }
        catch (DbUpdateException e)
        {
          return new BadRequestObjectResult(new BadRequestResource(
            $"Language ({language}): Name is already added "
          ));
        }
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpPut("{id}/Translations")]
    public async Task<IActionResult> UpdateBankTranslation([FromRoute] string id, [FromBody] BankTranslationResource bankTranslationResource)
    {
      if (ModelState.IsValid)
      {
        var bankTranslation = this.mapper.Map<BankTranslationResource, BankTranslation>(bankTranslationResource);

        var language = Request.Headers["Accept-Language"].ToString();
        await this.bankRepository.UpdateBankTranslation(id, bankTranslation, language);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
          $"Bank ({id}) updated"
        ));
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [HttpDelete("{id}/Translations")]
    public async Task<IActionResult> DeleteBankTranslation([FromRoute] string id, [FromQuery] string languageId)
    {
      if (ModelState.IsValid)
      {
        await this.bankRepository.DeleteBankTranslation(id, languageId);

        await this.unitOfWork.CompleteAsync();

        return new OkObjectResult(new OkResource(
          $"Bank ({id}) updated, ({languageId}) name deleted"
        ));
      }

      return new BadRequestObjectResult(new BadRequestResource(
        "Invalid request",
        ModelState.Keys
        .SelectMany(key => ModelState[key].Errors.Select
                      (x => new ValidationErrorResource(key, x.ErrorMessage)))
        .ToList()
      ));
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBank([FromRoute] string id)
    {
      var bank = await this.bankRepository.GetBankByIdAsync(id);
      if (bank == null)
      {
        return new NotFoundObjectResult(new NotFoundResource(
          $"Bank with Id ({id}) Not Found"
        ));
      }

      bank.IsActive = false;
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new OkResource(
        $"Bank with Id ({id}) has Disactivated"
      ));
    }
  }
}