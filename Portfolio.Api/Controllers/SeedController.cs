using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Data;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly PortfolioDbContext _db;

    public SeedController(PortfolioDbContext db)
    {
        _db = db;
    }

    // Dev utility: clears all data and restores the default seed state.
    [HttpPost("reset")]
    public async Task<IActionResult> Reset(CancellationToken ct)
    {
        await SeedRunner.ResetAsync(_db, ct);
        return NoContent();
    }
}
