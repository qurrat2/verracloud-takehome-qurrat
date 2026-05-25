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
}
