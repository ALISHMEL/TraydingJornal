using Microsoft.AspNetCore.Mvc;
using TradingJournal.Api.Services;
using TradingJournal.DTO;

namespace TradingJournal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly ITradeService _service;

    public TradesController(ITradeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trade = await _service.GetByIdAsync(id);

        if (trade == null)
            return NotFound();

        return Ok(trade);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTradeDto dto)
    {
        var trade = await _service.CreateAsync(dto);

        return Ok(trade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTradeDto dto)
    {
        var trade = await _service.UpdateAsync(id, dto);

        if (trade == null)
            return NotFound();

        return Ok(trade);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}