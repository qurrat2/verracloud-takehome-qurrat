using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Repositories;

public interface IPriceRepository
{
    Task AddRangeAsync(IEnumerable<Price> prices, CancellationToken ct = default);
    Task<List<Price>> ListHistoryAsync(IReadOnlyCollection<int> tickerIds, CancellationToken ct = default);
}
