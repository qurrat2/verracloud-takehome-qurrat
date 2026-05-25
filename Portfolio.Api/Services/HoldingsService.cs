using Portfolio.Api.Data.Entities;
using Portfolio.Api.Repositories;
using Portfolio.Api.Dtos;

namespace Portfolio.Api.Services;

public class HoldingsService : IHoldingsService
{
    private readonly IHoldingRepository _holdingRepo;
    private readonly ITickerRepository _tickerRepo;

    public HoldingsService(IHoldingRepository holdingRepo, ITickerRepository tickerRepo)
    {
        _holdingRepo = holdingRepo;
        _tickerRepo = tickerRepo;
    }

    public async Task<List<HoldingDto>> ListAsync(CancellationToken ct = default)
    {
        var holdings = await _holdingRepo.ListAsync(ct);

        return holdings.Select(h => new HoldingDto
        {
            Id = h.Id,
            Ticker = h.Ticker.Code,
            Quantity = h.Quantity,
            PurchasePrice = h.PurchasePrice,
            CurrentPrice = h.Ticker.CurrentPrice,
            MarketValue = decimal.Round(h.Quantity * h.Ticker.CurrentPrice, 4),
            UnrealizedPnL = decimal.Round((h.Ticker.CurrentPrice - h.PurchasePrice) * h.Quantity, 4),
            CreatedAt = h.CreatedAt
        }).ToList();
    }

    public async Task<HoldingDto> AddAsync(AddHoldingRequest request, CancellationToken ct = default)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.TickerCode))
            throw new ArgumentException("TickerCode is required", nameof(request.TickerCode));

        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(request.Quantity));

        if (request.PurchasePrice < 0)
            throw new ArgumentException("PurchasePrice must be non-negative", nameof(request.PurchasePrice));

        // Check ticker exists
        var ticker = await _tickerRepo.GetByCodeAsync(request.TickerCode.ToUpperInvariant(), ct);
        if (ticker == null)
            throw new ArgumentException($"Ticker '{request.TickerCode}' not found.");

        var holding = new Holding
        {
            TickerId = ticker.Id,
            Quantity = request.Quantity,
            PurchasePrice = request.PurchasePrice,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _holdingRepo.AddAsync(holding, ct);

        return new HoldingDto
        {
            Id = holding.Id,
            Ticker = ticker.Code,
            Quantity = holding.Quantity,
            PurchasePrice = holding.PurchasePrice,
            CurrentPrice = ticker.CurrentPrice,
            MarketValue = decimal.Round(holding.Quantity * ticker.CurrentPrice, 4),
            UnrealizedPnL = decimal.Round((ticker.CurrentPrice - holding.PurchasePrice) * holding.Quantity, 4),
            CreatedAt = holding.CreatedAt
        };
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var holding = await _holdingRepo.GetByIdAsync(id, ct);
        if (holding == null)
            throw new KeyNotFoundException("Holding not found");

        await _holdingRepo.DeleteAsync(holding, ct);
    }
}
