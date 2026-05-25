namespace Portfolio.Api.Data.Entities;

public class Price
{
    public int Id { get; set; }
    public int TickerId { get; set; }
    public Ticker Ticker { get; set; } = null!;
    public decimal Value { get; set; }
    public DateTimeOffset AsOf { get; set; }
}
