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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddAsync_throws_when_quantity_not_positive(decimal quantity)
    {
        var sut = new HoldingsService(new Mock<IHoldingRepository>().Object, new Mock<ITickerRepository>().Object);

        var req = new AddHoldingRequest { TickerCode = "AAPL", Quantity = quantity, PurchasePrice = 10m };

        await sut.Invoking(s => s.AddAsync(req))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("Quantity must be positive*");
    }

    [Fact]
    public async Task AddAsync_throws_when_purchase_price_negative()
    {
        var sut = new HoldingsService(new Mock<IHoldingRepository>().Object, new Mock<ITickerRepository>().Object);

        var req = new AddHoldingRequest { TickerCode = "AAPL", Quantity = 1m, PurchasePrice = -5m };

        await sut.Invoking(s => s.AddAsync(req))
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("PurchasePrice must be non-negative*");
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

    [Fact]
    public async Task ListAsync_calculates_market_value_and_pnl_for_gain()
    {
        var ticker = new Ticker { Id = 1, Code = "AAPL", Name = "Apple", CurrentPrice = 150m, LastUpdatedAt = DateTimeOffset.UtcNow };
        var holding = new Holding { Id = 10, TickerId = 1, Ticker = ticker, Quantity = 3m, PurchasePrice = 100m, CreatedAt = DateTimeOffset.UtcNow };

        var holdingRepo = new Mock<IHoldingRepository>();
        holdingRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Holding> { holding });

        var sut = new HoldingsService(holdingRepo.Object, new Mock<ITickerRepository>().Object);

        var result = await sut.ListAsync();

        result.Should().HaveCount(1);
        result[0].Ticker.Should().Be("AAPL");
        result[0].CurrentPrice.Should().Be(150m);
        result[0].MarketValue.Should().Be(450m);       // 3 � 150
        result[0].UnrealizedPnL.Should().Be(150m);     // (150 - 100) � 3
    }

    [Fact]
    public async Task ListAsync_returns_negative_pnl_when_current_below_purchase()
    {
        var ticker = new Ticker { Id = 1, Code = "MSFT", Name = "Microsoft", CurrentPrice = 150m, LastUpdatedAt = DateTimeOffset.UtcNow };
        var holding = new Holding { Id = 11, TickerId = 1, Ticker = ticker, Quantity = 2m, PurchasePrice = 200m, CreatedAt = DateTimeOffset.UtcNow };

        var holdingRepo = new Mock<IHoldingRepository>();
        holdingRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Holding> { holding });

        var sut = new HoldingsService(holdingRepo.Object, new Mock<ITickerRepository>().Object);

        var result = await sut.ListAsync();

        result[0].MarketValue.Should().Be(300m);       // 2 � 150
        result[0].UnrealizedPnL.Should().Be(-100m);    // (150 - 200) � 2  � sign must survive
    }

    [Fact]
    public async Task ListAsync_handles_fractional_quantity()
    {
        var ticker = new Ticker { Id = 1, Code = "GS", Name = "Goldman Sachs", CurrentPrice = 150m, LastUpdatedAt = DateTimeOffset.UtcNow };
        var holding = new Holding { Id = 12, TickerId = 1, Ticker = ticker, Quantity = 0.5m, PurchasePrice = 100m, CreatedAt = DateTimeOffset.UtcNow };

        var holdingRepo = new Mock<IHoldingRepository>();
        holdingRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Holding> { holding });

        var sut = new HoldingsService(holdingRepo.Object, new Mock<ITickerRepository>().Object);

        var result = await sut.ListAsync();

        result[0].MarketValue.Should().Be(75m);        // 0.5 � 150
        result[0].UnrealizedPnL.Should().Be(25m);      // (150 - 100) � 0.5  � fractional math intact
    }

    [Fact]
    public async Task ListAsync_returns_zero_market_value_when_ticker_has_no_price_yet()
    {
        // missing-price edge case: Ticker exists but refresh hasn't populated CurrentPrice yet (= 0m).
        // Pinned behavior: MarketValue is 0; PnL reflects the full unrealized loss of the purchase cost.
        var ticker = new Ticker { Id = 1, Code = "T", Name = "AT&T", CurrentPrice = 0m, LastUpdatedAt = DateTimeOffset.MinValue };
        var holding = new Holding { Id = 13, TickerId = 1, Ticker = ticker, Quantity = 4m, PurchasePrice = 25m, CreatedAt = DateTimeOffset.UtcNow };

        var holdingRepo = new Mock<IHoldingRepository>();
        holdingRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Holding> { holding });

        var sut = new HoldingsService(holdingRepo.Object, new Mock<ITickerRepository>().Object);

        var result = await sut.ListAsync();

        result[0].CurrentPrice.Should().Be(0m);
        result[0].MarketValue.Should().Be(0m);
        result[0].UnrealizedPnL.Should().Be(-100m);    // (0 - 25) � 4
    }
}
