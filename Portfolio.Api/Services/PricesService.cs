using Portfolio.Api.Data.Entities;
using Portfolio.Api.Dtos;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class PricesService : IPricesService
{
    private const decimal MaxMovePercent = 0.02m;

    private readonly ITickerRepository _tickerRepo;
    private readonly IPriceRepository _priceRepo;
    private readonly IHoldingRepository _holdingRepo;

    public PricesService(ITickerRepository tickerRepo, IPriceRepository priceRepo, IHoldingRepository holdingRepo)
    {
        _tickerRepo = tickerRepo;
        _priceRepo = priceRepo;
        _holdingRepo = holdingRepo;
    }

    public async Task<List<PriceDto>> ListAsync(CancellationToken ct = default)
    {
        var tickers = await _tickerRepo.ListAsync(ct);
        return tickers.Select(ToDto).ToList();
    }

    public async Task<List<PriceHistorySeriesDto>> GetHistoryAsync(CancellationToken ct = default)
    {
        var holdings = await _holdingRepo.ListAsync(ct);
        var tickerIds = holdings.Select(h => h.TickerId).Distinct().ToList();
        if (tickerIds.Count == 0)
            return new List<PriceHistorySeriesDto>();

        var history = await _priceRepo.ListHistoryAsync(tickerIds, ct);

        return history
            .GroupBy(p => p.Ticker.Code)
            .Select(g => new PriceHistorySeriesDto
            {
                Ticker = g.Key,
                Points = g.OrderBy(p => p.AsOf)
                    .Select(p => new PriceHistoryPointDto { AsOf = p.AsOf, Value = p.Value })
                    .ToList()
            })
            .OrderBy(s => s.Ticker)
            .ToList();
    }

    public async Task<List<PriceDto>> RefreshAsync(CancellationToken ct = default)
    {
        var tickers = await _tickerRepo.ListTrackedAsync(ct);
        var now = DateTimeOffset.UtcNow;
        var history = new List<Price>(tickers.Count);

        foreach (var ticker in tickers)
        {
            var newPrice = Randomize(ticker.CurrentPrice);
            ticker.CurrentPrice = newPrice;
            ticker.LastUpdatedAt = now;
            history.Add(new Price { TickerId = ticker.Id, Value = newPrice, AsOf = now });
        }

        // Tickers are tracked by the same scoped DbContext, so persisting the
        // history rows flushes the ticker price updates in the same transaction.
        await _priceRepo.AddRangeAsync(history, ct);

        return tickers.Select(ToDto).ToList();
    }

    private static decimal Randomize(decimal current)
    {
        var move = (decimal)(Random.Shared.NextDouble() * 2 - 1) * MaxMovePercent;
        return decimal.Round(current * (1 + move), 2);
    }

    private static PriceDto ToDto(Ticker t) => new()
    {
        TickerId = t.Id,
        Ticker = t.Code,
        Name = t.Name,
        CurrentPrice = t.CurrentPrice,
        LastUpdatedAt = t.LastUpdatedAt
    };
}
