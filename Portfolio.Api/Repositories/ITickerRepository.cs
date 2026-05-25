using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public interface ITickerRepository
{
    Task<Ticker?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Ticker?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<Ticker>> ListAsync(CancellationToken ct = default);
    Task<List<Ticker>> ListTrackedAsync(CancellationToken ct = default);
}
