namespace Portfolio.Api.Dtos;

public class PriceDto
{
    public string Ticker { get; set; } = null!;
    public decimal CurrentPrice { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
}
