namespace Portfolio.Api.Dtos;

public class PriceHistoryPointDto
{
    public DateTimeOffset AsOf { get; set; }
    public decimal Value { get; set; }
}

public class PriceHistorySeriesDto
{
    public string Ticker { get; set; } = null!;
    public List<PriceHistoryPointDto> Points { get; set; } = new();
}
