using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public class HoldingRepository : IHoldingRepository
{
    private readonly PortfolioDbContext _db;

    public HoldingRepository(PortfolioDbContext db)
    {
        _db = db;
    }

    public async Task<Holding?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Holdings.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<Holding>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Holdings
            .AsNoTracking()
            .Include(h => h.Ticker)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Holding holding, CancellationToken ct = default)
    {
        await _db.Holdings.AddAsync(holding, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Holding holding, CancellationToken ct = default)
    {
        _db.Holdings.Remove(holding);
        await _db.SaveChangesAsync(ct);
    }
}
