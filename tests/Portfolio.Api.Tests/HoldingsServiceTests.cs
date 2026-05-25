using System.Threading;
using FluentAssertions;
using Moq;
using Portfolio.Api.Data.Entities;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Dtos;
using Xunit;

namespace Portfolio.Api.Tests;

public class HoldingsServiceTests
{
    [Fact]
    public async Task AddAsync_throws_when_ticker_not_found()
    {
        var holdingRepo = new Mock<IHoldingRepository>();
        var tickerRepo = new Mock<ITickerRepository>();
        tickerRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticker?)null);

        var sut = new HoldingsService(holdingRepo.Object, tickerRepo.Object);

        var req = new AddHoldingRequest { TickerCode = "XYZ", Quantity = 1m, PurchasePrice = 10m };

        await sut.Invoking(s => s.AddAsync(req)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_creates_holding_and_returns_dto()
    {
        var holdingRepo = new Mock<IHoldingRepository>();
        holdingRepo.Setup(r => r.AddAsync(It.IsAny<Holding>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<Holding, CancellationToken>((h, ct) => { h.Id = 42; });

        var ticker = new Ticker { Id = 1, Code = "AAPL", CurrentPrice = 150m, LastUpdatedAt = DateTimeOffset.UtcNow, Name = "Apple" };
        var tickerRepo = new Mock<ITickerRepository>();
        tickerRepo.Setup(r => r.GetByCodeAsync("AAPL", It.IsAny<CancellationToken>())).ReturnsAsync(ticker);

        var sut = new HoldingsService(holdingRepo.Object, tickerRepo.Object);

        var req = new AddHoldingRequest { TickerCode = "AAPL", Quantity = 2m, PurchasePrice = 100m };

        var dto = await sut.AddAsync(req);

        dto.Id.Should().Be(42);
        dto.Ticker.Should().Be("AAPL");
        dto.Quantity.Should().Be(2m);
        dto.PurchasePrice.Should().Be(100m);
        dto.CurrentPrice.Should().Be(150m);
        dto.MarketValue.Should().Be(300m);
        dto.UnrealizedPnL.Should().Be(100m);
    }
}
