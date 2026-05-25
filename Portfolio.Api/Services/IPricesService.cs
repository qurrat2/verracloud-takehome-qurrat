using Portfolio.Api.Dtos;

namespace Portfolio.Api.Services;

public interface IPricesService
{
    Task<List<PriceDto>> ListAsync(CancellationToken ct = default);
    Task<List<PriceDto>> RefreshAsync(CancellationToken ct = default);
}
