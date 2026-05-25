namespace Portfolio.Api.Data.Entities;

public class Ticker
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
}