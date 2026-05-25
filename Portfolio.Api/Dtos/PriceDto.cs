namespace Portfolio.Api.Dtos;

public class PriceDto
{
    public int TickerId { get; set; }
    public string Ticker { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal CurrentPrice { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
}
