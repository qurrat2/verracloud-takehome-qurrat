namespace Portfolio.Api.Dtos;

public class AddHoldingRequest
{
    public string TickerCode { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
}