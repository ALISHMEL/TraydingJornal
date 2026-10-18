using Microsoft.AspNetCore.Mvc;
using TradingJournal.Api.Services;
using TradingJournal.Bybit;

namespace TradingJournal.Api.Controllers;

[ApiController]
[Route("api/bybit")]
public class BybitController : ControllerBase
{
    private readonly IBybitService _service;
    private readonly IImportService _importService;
    public BybitController(
    IBybitService service,
    IImportService importService)
    {
        _service = service;
        _importService = importService;
    }


    [HttpGet("time")]
    public async Task<IActionResult> Time()
    {
        return Ok(await _service.GetServerTimeAsync());
    }
    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        var result = await _importService.ImportHistoryAsync();

        return Ok(result);
    }
}