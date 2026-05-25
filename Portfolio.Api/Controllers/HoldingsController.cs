using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Api.Dtos;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoldingsController : ControllerBase
{
    private readonly IHoldingsService _service;

    public HoldingsController(IHoldingsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<HoldingDto>> Get(CancellationToken ct)
    {
        return await _service.ListAsync(ct);
    }

    [HttpPost]
    public async Task<ActionResult<HoldingDto>> Post(AddHoldingRequest req, CancellationToken ct)
    {
        var dto = await _service.AddAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
