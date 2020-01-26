using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApp1.Controllers.Resources;
using WebApp1.Core.Models;
using WebApp1.Hubs;
using WebApp1.Persistence;

namespace WebApp1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : Controller
  {
    private readonly ApplicationDbContext context;
    private readonly IHubContext<ValuesHub> hubContext;
    private readonly IMapper mapper;
    public ValuesController(ApplicationDbContext context, IHubContext<ValuesHub> hubContext, IMapper mapper)
    {
      this.mapper = mapper;
      this.hubContext = hubContext;
      this.context = context;

    }
    [HttpGet]
    public async Task<IEnumerable<ValueResource>> GetValues()
    {
      var values = await this.context.Values.ToListAsync();
      var result = this.mapper.Map<IEnumerable<Value>, IEnumerable<ValueResource>>(values);
      return result;
    }

    [HttpPost]
    public IActionResult AddValue([FromBody] ValueResource valueResource)
    {
      var value = this.mapper.Map<ValueResource, Value>(valueResource);

      this.context.Add(value);
      this.context.SaveChanges();

      //   this.hubContext.Clients.All.SendAsync("Add", value);

      return new OkObjectResult("value added");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteValue(int id)
    {
      var value = this.context.Values.Where(v => v.Id == id);

      this.context.Remove(value);
      this.context.SaveChanges();

      //   this.hubContext.Clients.All.SendAsync("Delete", value);

      return new OkObjectResult("value added");
    }
  }
}