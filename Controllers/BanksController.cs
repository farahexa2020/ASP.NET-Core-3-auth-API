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

namespace WebApp1.Controllers
{
  [Route("api/banks")]
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
    public async Task<IActionResult> GetActiveBanks()
    {
      var language = Request.Headers["Accept-Language"].ToString();

      var banks = await this.bankRepository.GetActiveBanksAsync();
      var result = this.mapper.Map<IEnumerable<Bank>, IEnumerable<BankResource>>(banks, opt => opt.Items["language"] = language);

      return new OkObjectResult(new OkResource(
        "All Active Banks",
        result
      ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBankById(int id)
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
      bank.UpdatedAt = DateTime.Now;
      bank.UpdatedBy = userId;

      this.bankRepository.Add(bank);
      await this.unitOfWork.CompleteAsync();

      return new OkObjectResult(new CreatedResource(
        "Bank created"
      ));
    }

    [HttpPost("{id}/Translations")]
    public async Task<IActionResult> AddBankTranslation([FromRoute] int id, [FromBody] BankTranslation bankTranslation)
    {
      if (ModelState.IsValid)
      {
        var language = Request.Headers["Accept-Language"].ToString();
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
            $"Language ({language}): Name ({bankTranslation.Name}) is already added "
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

    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBank([FromRoute] int id)
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