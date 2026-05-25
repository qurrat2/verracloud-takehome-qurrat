using System.Threading;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Data;

public static class SeedRunner
{
    private const int HistoryPoints = 24;
    private static readonly TimeSpan HistoryInterval = TimeSpan.FromMinutes(5);
    private const decimal MaxMovePercent = 0.02m;

    private static readonly (string Code, string Name, decimal Price)[] TickerSeeds =
    {
        ("AAPL", "Apple Inc.", 150.00m),
        ("MSFT", "Microsoft Corporation", 300.00m),
        ("JPM", "JPMorgan Chase & Co.", 100.00m),
        ("T", "AT&T Inc.", 18.00m),
        ("GS", "Goldman Sachs Group, Inc.", 350.00m),
        ("GOOGL", "Alphabet Inc.", 140.00m),
        ("AMZN", "Amazon.com, Inc.", 135.00m),
        ("NVDA", "NVIDIA Corporation", 450.00m),
        ("TSLA", "Tesla, Inc.", 250.00m),
        ("META", "Meta Platforms, Inc.", 320.00m)
    };

    private static readonly (string Code, decimal Quantity, decimal PurchasePrice)[] HoldingSeeds =
    {
        ("AAPL", 10m, 145.00m),
        ("MSFT", 5m, 290.00m),
        ("NVDA", 8m, 400.00m),
        ("GOOGL", 12m, 138.00m),
        ("TSLA", 4m, 270.00m)
    };

    public static async Task SeedAsync(PortfolioDbContext db, CancellationToken ct = default)
    {
        await SeedTickersAsync(db, ct);

        var tickers = await db.Tickers.ToListAsync(ct);
        var byCode = tickers.ToDictionary(t => t.Code);

        await SeedHoldingsAsync(db, byCode, ct);
        await SeedPriceHistoryAsync(db, tickers, ct);
    }

    // Dev utility: wipe everything and rebuild the seed state. Children
    // (Holdings, Prices) are deleted before Tickers to respect the foreign keys.
    public static async Task ResetAsync(PortfolioDbContext db, CancellationToken ct = default)
    {
        await db.Holdings.ExecuteDeleteAsync(ct);
        await db.Prices.ExecuteDeleteAsync(ct);
        await db.Tickers.ExecuteDeleteAsync(ct);
        await SeedAsync(db, ct);
    }

    private static async Task SeedTickersAsync(PortfolioDbContext db, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var (code, name, price) in TickerSeeds)
        {
            ct.ThrowIfCancellationRequested();
            var exists = await db.Tickers.AsNoTracking().AnyAsync(t => t.Code == code, ct);
            if (!exists)
            {
                await db.Tickers.AddAsync(
                    new Ticker { Code = code, Name = name, Description = "", CurrentPrice = price, LastUpdatedAt = now },
                    ct);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedHoldingsAsync(PortfolioDbContext db, IReadOnlyDictionary<string, Ticker> byCode, CancellationToken ct)
    {
        if (await db.Holdings.AnyAsync(ct))
            return;

        var createdAt = DateTimeOffset.UtcNow;
        foreach (var (code, quantity, purchasePrice) in HoldingSeeds)
        {
            if (!byCode.TryGetValue(code, out var ticker))
                continue;

            await db.Holdings.AddAsync(
                new Holding { TickerId = ticker.Id, Quantity = quantity, PurchasePrice = purchasePrice, CreatedAt = createdAt },
                ct);
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedPriceHistoryAsync(PortfolioDbContext db, IReadOnlyList<Ticker> tickers, CancellationToken ct)
    {
        if (await db.Prices.AnyAsync(ct))
            return;

        var now = DateTimeOffset.UtcNow;
        var history = new List<Price>(tickers.Count * HistoryPoints);

        foreach (var ticker in tickers)
        {
            // Build a back-dated random walk that ends at the ticker's current price.
            var values = new decimal[HistoryPoints];
            values[HistoryPoints - 1] = ticker.CurrentPrice;
            for (var i = HistoryPoints - 2; i >= 0; i--)
            {
                var move = (decimal)(Random.Shared.NextDouble() * 2 - 1) * MaxMovePercent;
                values[i] = decimal.Round(values[i + 1] / (1 + move), 2);
            }

            for (var i = 0; i < HistoryPoints; i++)
            {
                history.Add(new Price
                {
                    TickerId = ticker.Id,
                    Value = values[i],
                    AsOf = now - HistoryInterval * (HistoryPoints - 1 - i)
                });
            }
        }

        await db.Prices.AddRangeAsync(history, ct);
        await db.SaveChangesAsync(ct);
    }
}
