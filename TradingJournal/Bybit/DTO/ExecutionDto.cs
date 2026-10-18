namespace TradingJournal.Api.Bybit.DTO;

public class ExecutionDto
{
    public string ExecutionId { get; set; } = "";

    public string OrderId { get; set; } = "";

    public string Symbol { get; set; } = "";

    public string Side { get; set; } = "";

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Fee { get; set; }

    public DateTime Time { get; set; }
}