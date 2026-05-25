using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly PortfolioDbContext _db;

    public PriceRepository(PortfolioDbContext db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(IEnumerable<Price> prices, CancellationToken ct = default)
    {
        await _db.Prices.AddRangeAsync(prices, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<Price>> ListHistoryAsync(IReadOnlyCollection<int> tickerIds, CancellationToken ct = default)
    {
        // No server-side OrderBy: SQLite can't translate ORDER BY on DateTimeOffset.
        // PricesService sorts each ticker's points by AsOf in memory.
        return await _db.Prices
            .AsNoTracking()
            .Include(p => p.Ticker)
            .Where(p => tickerIds.Contains(p.TickerId))
            .ToListAsync(ct);
    }
}
