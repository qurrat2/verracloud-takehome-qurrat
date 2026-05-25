using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public class TickerRepository : ITickerRepository
{
    private readonly PortfolioDbContext _db;

    public TickerRepository(PortfolioDbContext db)
    {
        _db = db;
    }

    public async Task<Ticker?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Tickers.FindAsync(new object[] { id }, ct);
    }

    public async Task<Ticker?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _db.Tickers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == code, ct);
    }

    public async Task<List<Ticker>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Tickers
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<Ticker>> ListTrackedAsync(CancellationToken ct = default)
    {
        return await _db.Tickers.ToListAsync(ct);
    }
}
