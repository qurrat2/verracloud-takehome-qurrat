using Portfolio.Api.Dtos;

namespace Portfolio.Api.Services;

public interface IHoldingsService
{
    Task<List<HoldingDto>> ListAsync(CancellationToken ct = default);
    Task<HoldingDto> AddAsync(AddHoldingRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}