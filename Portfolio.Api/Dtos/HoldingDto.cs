namespace Portfolio.Api.Dtos;

public class HoldingDto
{
    public int Id { get; set; }
    public string Ticker { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal MarketValue { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}