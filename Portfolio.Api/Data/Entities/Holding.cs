namespace Portfolio.Api.Data.Entities;

public class Holding
{
    public int Id { get; set; }
    public int TickerId { get; set; }
    public Ticker Ticker { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}