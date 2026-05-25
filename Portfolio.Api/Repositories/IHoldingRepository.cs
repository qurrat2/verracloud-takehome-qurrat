using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public interface IHoldingRepository
{
    Task<Holding?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Holding>> ListAsync(CancellationToken ct = default);
    Task AddAsync(Holding holding, CancellationToken ct = default);
    Task DeleteAsync(Holding holding, CancellationToken ct = default);
}
