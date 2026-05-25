using System.Threading;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data.Entities;

namespace Portfolio.Api.Data;

public static class SeedRunner
{
    public static async Task SeedAsync(PortfolioDbContext db, CancellationToken ct = default)
    {
        var seeds = new[]
        {
            new Ticker { Code = "AAPL", Name = "Apple Inc.", Description = "", CurrentPrice = 150.00m, LastUpdatedAt = DateTimeOffset.UtcNow },
            new Ticker { Code = "MSFT", Name = "Microsoft Corporation", Description = "", CurrentPrice = 300.00m, LastUpdatedAt = DateTimeOffset.UtcNow },
            new Ticker { Code = "JPM", Name = "JPMorgan Chase & Co.", Description = "", CurrentPrice = 100.00m, LastUpdatedAt = DateTimeOffset.UtcNow },
            new Ticker { Code = "T", Name = "AT&T Inc.", Description = "", CurrentPrice = 18.00m, LastUpdatedAt = DateTimeOffset.UtcNow },
            new Ticker { Code = "GS", Name = "Goldman Sachs Group, Inc.", Description = "", CurrentPrice = 350.00m, LastUpdatedAt = DateTimeOffset.UtcNow }
        };

        foreach (var seed in seeds)
        {
            ct.ThrowIfCancellationRequested();

            var exists = await db.Tickers
                .AsNoTracking()
                .AnyAsync(t => t.Code == seed.Code, ct);

            if (!exists)
            {
                await db.Tickers.AddAsync(seed, ct);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}