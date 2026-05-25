using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPricesService _service;

    public PricesController(IPricesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<PriceDto>> Get(CancellationToken ct)
    {
        return await _service.ListAsync(ct);
    }

    [HttpPost("refresh")]
    public async Task<IEnumerable<PriceDto>> Refresh(CancellationToken ct)
    {
        return await _service.RefreshAsync(ct);
    }
}
